using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Models
{
    public class TextCryptographyModel
    {
        public string PlainText { get; set; }
        public string CryptedText { get; set; }
        public string CryptedTextIV { get; set; }
    }
}
