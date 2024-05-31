namespace Roma_Shop_Server.Models
{
    public class Order
    {
        public string Id { get; set; }
        public ICollection<OrderItem> Products { get; set; }
        public string Address { get; set; }
        public string Status { get; set; } = "Не собран";
        public string UserId { get; set; }
    }
}
