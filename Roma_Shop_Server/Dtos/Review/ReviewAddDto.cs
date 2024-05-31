namespace Roma_Shop_Server.Dtos.Review
{
    public class ReviewCreateDto
    {
        public string ReviewText { get; set; }
        public int Rating { get; set; } = 1;
    }
}
