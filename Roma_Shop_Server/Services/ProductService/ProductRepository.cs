using Microsoft.EntityFrameworkCore;
using Roma_Shop_Server.Dtos.Product;
using Roma_Shop_Server.Models;
using Roma_Shop_Server.Models.Data;
using Roma_Shop_Server.Models.DB;

namespace Roma_Shop_Server.Services.ProductService
{
    public class ProductRepository
    {
        private ApplicationContext _context;

        public ProductRepository(ApplicationContext reviewContext)
        {
            _context = reviewContext;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(string id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<ServiceResponse<Product>> Add(ProductCreateDTO productCreateDTO)
        {
            ServiceResponse<Product> response = new ServiceResponse<Product>();

            try
            {
                var newProduct = new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = productCreateDTO.Title,
                    Price = productCreateDTO.Price,
                    Category = productCreateDTO.Category,
                    imgUrl = productCreateDTO.ImgUrl,
                    Reviews = new List<Review>([])
                };

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                response.Data = newProduct;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> AddRange(ICollection<ProductCreateDTO> productCreateDTOList)
        {
            ServiceResponse<List<Product>> response = new ServiceResponse<List<Product>>();

            try
            {
                var newProducts = productCreateDTOList.Select(productCreateDTO => new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = productCreateDTO.Title,
                    Price = productCreateDTO.Price,
                    Category = productCreateDTO.Category,
                    imgUrl = productCreateDTO.ImgUrl,
                    Reviews = new List<Review>([])
                }).ToList();

                _context.Products.AddRange(newProducts);
                await _context.SaveChangesAsync();

                response.Data = newProducts;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteProduct(string id)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product not found.";
                    return response;
                }

                _context.Products.Remove(product);
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
