using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Models
{
    public class UserPasswords
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordIV { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string NewPasswordIV { get; set; }
    }
}
