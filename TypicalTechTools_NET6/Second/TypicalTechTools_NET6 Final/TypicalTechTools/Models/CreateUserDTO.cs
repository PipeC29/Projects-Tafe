namespace TypicalTechTools.Models
{
    public class CreateUserDTO : LoginUserDTO
    {
        public string PasswordConfirmation { get; set; }
    }
}
