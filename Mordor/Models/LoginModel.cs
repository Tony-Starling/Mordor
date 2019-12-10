using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mordor.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "EmptyUserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "EmptyPassword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
