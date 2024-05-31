using System.ComponentModel.DataAnnotations;

namespace Roma_Shop_Server.Dtos.User
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress()]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^(?=.*[a-zA-Z])[\w]{8,}$")]
        public string Password { get; set; } = string.Empty;
    }
}
