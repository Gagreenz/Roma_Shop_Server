using System.ComponentModel.DataAnnotations;

namespace Roma_Shop_Server.Dtos.User
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress()]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone()]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")]
        public string Password { get; set; } = string.Empty;
    }
}
