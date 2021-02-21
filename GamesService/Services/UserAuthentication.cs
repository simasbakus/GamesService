using GamesService.Models;
using GamesService.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace GamesService.Services
{
    public class UserAuthentication : IUserAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoRepository _repository;

        public UserAuthentication(IConfiguration configuration, IMongoRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public string Authenticate(UserCred userCred)
        {
            User foundUser;

            string username = DecryptCredential(userCred.Username, userCred.UsernameIV);
            string password = DecryptCredential(userCred.Password, userCred.PasswordIV);

            try
            {
                foundUser = _repository.GetUserByUsername(username);
            }
            catch (Exception)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, foundUser.Password))
                return null;

            return GenerateJwtToken(foundUser);
        }

        private string GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.ASCII.GetBytes(_configuration["SecretKey"]);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("Id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string DecryptCredential(string credential, string IV)
        {
            Aes cipher = Aes.Create();
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = Encoding.ASCII.GetBytes(_configuration["CryptographyKey"]);

            cipher.IV = Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] ciphertext = Convert.FromBase64String(credential);
            byte[] plaintext = cryptTransform.TransformFinalBlock(ciphertext, 0, ciphertext.Length);

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
