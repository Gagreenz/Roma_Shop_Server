using Roma_Shop_Server.Models.Data;

namespace Roma_Shop_Server.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte[] PasswordHash { get; set; } = default!;
        public byte[] PasswordSalt { get; set; } = default!;
        public string? Role { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
