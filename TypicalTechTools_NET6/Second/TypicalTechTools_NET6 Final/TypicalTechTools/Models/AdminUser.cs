using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a user name")]

        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string PasswordHarsh { get; set; }

        public bool IsAdmin = true;
    }
}
