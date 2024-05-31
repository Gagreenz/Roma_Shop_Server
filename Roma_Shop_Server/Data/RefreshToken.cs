using System.ComponentModel.DataAnnotations.Schema;

namespace Roma_Shop_Server.Models.Data
{
    public class RefreshToken
    {
        public string Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
