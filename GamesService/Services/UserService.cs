using GamesService.Entities;
using GamesService.Models;
using GamesService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GamesService.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _repositoryUsers;
        private readonly IRefreshTokensRepository _repositoryRefreshTokens;
        private readonly ICryptographyService _cryptography;

        public UserService(IConfiguration configuration, IUsersRepository repositoryUsers, ICryptographyService cryptography, 
                            IRefreshTokensRepository repositoryRefreshTokens)
        {
            _configuration = configuration;
            _repositoryUsers = repositoryUsers;
            _cryptography = cryptography;
            _repositoryRefreshTokens = repositoryRefreshTokens;
        }

        public async Task<AuthenticationResponse> Authenticate(UserCred userCred)
        {
            User foundUser;

            try
            {
                foundUser = await _repositoryUsers.GetUserByUsername(_cryptography.DecryptCredential(userCred.Username, userCred.UsernameIV));
            }
            catch (Exception)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(_cryptography.DecryptCredential(userCred.Password, userCred.PasswordIV), foundUser.Password))
                return null;

            return await GenerateAuthResponse(foundUser);
        }

        public async Task<bool> ChangePassword(UserPasswords userPasswords, string userId)
        {
            User foundUser;

            try
            {
                foundUser = await _repositoryUsers.GetUserById(userId);
            }
            catch (Exception)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(_cryptography.DecryptCredential(userPasswords.Password, userPasswords.PasswordIV), foundUser.Password))
                return false;

            try
            {
                await _repositoryUsers.ChangePassword(foundUser, BCrypt.Net.BCrypt.HashPassword(_cryptography.DecryptCredential(userPasswords.NewPassword, userPasswords.NewPasswordIV)));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<AuthenticationResponse> RefreshTokens(string token, string refreshToken)
        {
            ClaimsPrincipal validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
                return null;

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            RefreshToken storedRefreshToken;

            try
            {
                storedRefreshToken = await _repositoryRefreshTokens.GetRefreshToken(refreshToken);

                if (storedRefreshToken == null)
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

            // Check if refreshToken is not expired, not invalidated, not used and its JwtId matches the id of Jwt token

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate
                || storedRefreshToken.IsInvalidated
                || storedRefreshToken.IsUsed
                || storedRefreshToken.JwtId != jti)
                return null;

            User user = await _repositoryUsers.GetUserById(validatedToken.Claims.Single(x => x.Type == "UserId").Value);

            try
            {
                await _repositoryRefreshTokens.UseRefreshToken(storedRefreshToken);
            }
            catch (Exception)
            {
                return null;
            }

            return await GenerateAuthResponse(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            TokenValidationParameters validationParams = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["SecretKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler tokenHandler = new();
            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParams, out var validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken 
                    && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResponse> GenerateAuthResponse(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] tokenKey = Encoding.ASCII.GetBytes(_configuration["SecretKey"]);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            RefreshToken newRefreshToken = new()
            {
                UserId = user.Id.ToString(),
                JwtId = token.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsUsed = false,
                IsInvalidated = false
            };

            await _repositoryRefreshTokens.CreateRefreshToken(newRefreshToken);

            return new AuthenticationResponse()
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = newRefreshToken.Token.ToString()
            };
        }
    }
}
