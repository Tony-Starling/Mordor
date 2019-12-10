using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Mordor.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "EmptyUserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "EmptyEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "EmptyPassword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
