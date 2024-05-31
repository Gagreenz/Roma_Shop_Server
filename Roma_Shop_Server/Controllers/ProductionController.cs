using Microsoft.AspNetCore.Mvc;
using Roma_Shop_Server.Dtos.Product;
using Roma_Shop_Server.Services.ProductService;

namespace Roma_Shop_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        public ProductionController(ProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }


        [HttpGet("showcase")]
        public async Task<IActionResult> Get()
        {
            var products = await _productRepository.GetAllProducts();
            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Post([FromBody] ProductCreateDTO product)
        {
            if (product == null)
            {
                return BadRequest("The product data is null.");
            }

            var response = await _productRepository.Add(product);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return CreatedAtAction(nameof(Get), new { id = response.Data.Id }, response.Data);
        }

        [HttpPost("AddRange")]
        public async Task<IActionResult> PostRange([FromBody] ICollection<ProductCreateDTO> products)
        {
            if (products == null || !products.Any())
            {
                return BadRequest("The products collection is null or empty.");
            }

            var response = await _productRepository.AddRange(products);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return CreatedAtAction(nameof(Get), response.Data);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _productRepository.DeleteProduct(id);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return NoContent();
        }


    }
}



