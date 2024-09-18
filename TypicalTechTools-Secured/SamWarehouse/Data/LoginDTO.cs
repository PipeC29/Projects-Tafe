using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApp.Data
{
    [NotMapped] 
    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; } 
        public string PasswordConfirmation { get; set; }
        public string Role { get; set; }
        public string ReturnUrl { get; set; }

        
    }
}
