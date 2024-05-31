namespace Roma_Shop_Server.Models
{
    public class Review
    {
        public string Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public bool Approved { get; set; } = false;
    }
}
