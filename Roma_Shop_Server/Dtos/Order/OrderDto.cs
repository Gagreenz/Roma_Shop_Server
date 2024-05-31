namespace Roma_Shop_Server.Dtos.Order
{
    public class OrderDto
    {
        public string UserId { get; set; }
        public string Address { get; set; }
        public ICollection<OrderItemDto> Products { get; set; }
    }
}
