using Microsoft.EntityFrameworkCore;
using Roma_Shop_Server.Dtos.Review;
using Roma_Shop_Server.Models;
using Roma_Shop_Server.Models.Data;
using Roma_Shop_Server.Models.DB;

namespace Roma_Shop_Server.Services.ReviewService
{
    public class ReviewRepository
    {
        private readonly ApplicationContext _context;

        public ReviewRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Review>> AddReview(string productId, ReviewCreateDto reviewDTO)
        {
            ServiceResponse<Review> response = new ServiceResponse<Review>();

            try
            {
                var newReview = new Review
                {
                    Id = Guid.NewGuid().ToString(),
                    ReviewText = reviewDTO.ReviewText,
                    Rating = reviewDTO.Rating,
                    ProductId = productId,
                    Approved = false
                };

                _context.Reviews.Add(newReview);
                await _context.SaveChangesAsync();

                response.Data = newReview;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ApproveReview(string reviewId)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Review not found.";
                    return response;
                }

                review.Approved = true;
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

        public async Task<double> GetAverageRatingByProductId(string productId)
        {
            return await _context.Reviews
                .Where(review => review.ProductId == productId && review.Approved)
                .AverageAsync(review => review.Rating);
        }

        public async Task<ServiceResponse<List<Review>>> GetUnapprovedReviews()
        {
            ServiceResponse<List<Review>> response = new ServiceResponse<List<Review>>();

            try
            {
                response.Data = await _context.Reviews
                    .Where(review => !review.Approved)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<Review>>> GetApprovedReviewsByProductId(string productId)
        {
            ServiceResponse<List<Review>> response = new ServiceResponse<List<Review>>();

            try
            {
                response.Data = await _context.Reviews
                    .Where(review => review.ProductId == productId && review.Approved)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteReview(string reviewId)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Review not found.";
                    return response;
                }

                _context.Reviews.Remove(review);
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