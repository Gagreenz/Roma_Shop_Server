namespace Roma_Shop_Server.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public float Price { get; set; }
        public string Category { get; set; }
        public string imgUrl { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
