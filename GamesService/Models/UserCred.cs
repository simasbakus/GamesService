using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamesService.Models
{
    public class UserCred
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string UsernameIV { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordIV { get; set; }
    }
}
