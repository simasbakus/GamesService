using GamesService.Models;
using GamesService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ICryptographyService _cryptography;

        public SettingsController(ICryptographyService cryptography)
        {
            _cryptography = cryptography;
        }

        [HttpGet("PingTest")]
        public IActionResult PingTest()
        {
            return Ok("Ping Test Successful!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("TestEncryption")]
        public IActionResult Encrypt([FromBody] TextCryptographyModel text)
        {
            (string, string) result = _cryptography.EncryptCredential(text.PlainText);

            return Ok(new TextCryptographyModel() { CryptedText = result.Item1, CryptedTextIV = result.Item2 });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("TestDecryption")]
        public IActionResult Decrypt([FromBody] TextCryptographyModel text)
        {
            string result = _cryptography.DecryptCredential(text.CryptedText, text.CryptedTextIV);

            return Ok(new TextCryptographyModel() { PlainText = result });
        }
    }
}
