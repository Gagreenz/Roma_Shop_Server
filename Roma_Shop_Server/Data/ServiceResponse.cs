namespace Roma_Shop_Server.Models.Data
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; } = true;
        public T? Data { get; set; } = default(T?);
        public string Message { get; set; } = string.Empty;
    }
}