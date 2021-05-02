using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GamesService.Services
{
    public class CryptographyService : ICryptographyService
    {
        private readonly IConfiguration _configuration;

        public CryptographyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DecryptCredential(string credential, string IV)
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

        public (string, string) EncryptCredential(string credential)
        {
            Aes cipher = Aes.Create();
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = Encoding.ASCII.GetBytes(_configuration["CryptographyKey"]);

            string IV = Convert.ToBase64String(cipher.IV);

            ICryptoTransform cryptoTransform = cipher.CreateEncryptor();
            byte[] plainText = Encoding.ASCII.GetBytes(credential);
            byte[] cypherText = cryptoTransform.TransformFinalBlock(plainText, 0, plainText.Length);

            return ( Convert.ToBase64String(cypherText), IV );
        }
    }
}
