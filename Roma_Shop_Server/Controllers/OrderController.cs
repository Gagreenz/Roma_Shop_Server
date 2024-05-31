using Microsoft.AspNetCore.Mvc;
using Roma_Shop_Server.Dtos.Order;
using Roma_Shop_Server.Models;
using Roma_Shop_Server.Services.OrderService;

namespace Roma_Shop_Server.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;

        public OrderController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderRepository.GetOrderById(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();

            return Ok(orders);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            var response = await _orderRepository.CreateOrder(orderDto);

            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(response.Message);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var response = await _orderRepository.DeleteOrder(id);

            if (response.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(response.Message);
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateStatusDto updateStatusDto)
        {
            var response = await _orderRepository.UpdateOrderStatus(id, updateStatusDto.NewStatus);

            if (response.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(response.Message);
        }

        [HttpPut("get-order-by-userid/{id}")]
        public async Task<IActionResult> GetByUserId(string id)
        {
            var response = await _orderRepository.GetOrdersByUserId(id);

            if (response.Count == 0) response = [];

            return Ok(response);
        }
    }
}
