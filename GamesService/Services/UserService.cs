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
        private readonly IUsersRepository _repository;
        private readonly ICryptographyService _cryptography;

        public UserService(IConfiguration configuration, IUsersRepository repository, ICryptographyService cryptography)
        {
            _configuration = configuration;
            _repository = repository;
            _cryptography = cryptography;
        }

        public async Task<string> Authenticate(UserCred userCred)
        {
            User foundUser;

            try
            {
                foundUser = await _repository.GetUserByUsername(_cryptography.DecryptCredential(userCred.Username, userCred.UsernameIV));
            }
            catch (Exception)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(_cryptography.DecryptCredential(userCred.Password, userCred.PasswordIV), foundUser.Password))
                return null;

            return GenerateJwtToken(foundUser);
        }

        public async Task<bool> ChangePassword(UserPasswords userPasswords, string userId)
        {
            User foundUser;

            try
            {
                foundUser = await _repository.GetUserById(userId);
            }
            catch (Exception)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(_cryptography.DecryptCredential(userPasswords.Password, userPasswords.PasswordIV), foundUser.Password))
                return false;

            try
            {
                await _repository.ChangePassword(foundUser, BCrypt.Net.BCrypt.HashPassword(_cryptography.DecryptCredential(userPasswords.NewPassword, userPasswords.NewPasswordIV)));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] tokenKey = Encoding.ASCII.GetBytes(_configuration["SecretKey"]);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[] { new Claim("Id", user.Id.ToString()), new Claim(ClaimTypes.Role, user.Role) }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
