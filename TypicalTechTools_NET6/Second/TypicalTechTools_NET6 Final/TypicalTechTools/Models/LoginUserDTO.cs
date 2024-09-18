using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models
{
    public class LoginUserDTO
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
