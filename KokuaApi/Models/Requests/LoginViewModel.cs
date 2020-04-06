using System;
using System.ComponentModel.DataAnnotations;

namespace KokuaApi.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Grant_Type { get; set; } = "password";
    }
}
