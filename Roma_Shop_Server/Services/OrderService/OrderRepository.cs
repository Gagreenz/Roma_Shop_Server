using Microsoft.EntityFrameworkCore;
using Roma_Shop_Server.Dtos.Order;
using Roma_Shop_Server.Models;
using Roma_Shop_Server.Models.Data;
using Roma_Shop_Server.Models.DB;

namespace Roma_Shop_Server.Services.OrderService
{
    public class OrderRepository
    {
        private ApplicationContext _context;

        public OrderRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.Orders.Include(o => o.Products).ToListAsync();
        }

        public async Task<Order> GetOrderById(string id)
        {
            return await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Order>> GetOrdersByUserId(string id)
        {
            return await _context.Orders
                .Include(o => o.Products)
                .Where(o => o.UserId == id)
                .ToListAsync();
        }
        public async Task<ServiceResponse<Order>> CreateOrder(OrderDto orderDto)
        {
            ServiceResponse<Order> response = new ServiceResponse<Order>();

            try
            {
                var order = new Order
                {
                    UserId = orderDto.UserId,
                    Id = Guid.NewGuid().ToString(),
                    Address = orderDto.Address,
                    Products = orderDto.Products.Select(p => new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = p.Title,
                        Count = p.Count,
                        Price = p.Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                response.Data = order;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteOrder(string id)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            try
            {
                var order = await _context.Orders
                                           .Include(o => o.Products)
                                           .FirstOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                    return response;
                }

                _context.OrderItems.RemoveRange(order.Products);
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();

                response.Data = true;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = false;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateOrderStatus(string id, string newStatus)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                    return response;
                }

                order.Status = newStatus;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                response.Data = true;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = false;
            }

            return response;
        }
    }
}
