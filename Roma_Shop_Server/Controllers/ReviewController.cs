using Microsoft.AspNetCore.Mvc;
using Roma_Shop_Server.Dtos.Review;
using Roma_Shop_Server.Services.ReviewService;

namespace Roma_Shop_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;

        public ReviewController(ReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDto reviewDTO, string productId)
        {
            var response = await _reviewRepository.AddReview(productId, reviewDTO);

            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(response);
        }

        [HttpPost("approve/{reviewId}")]
        public async Task<IActionResult> ApproveReview(string reviewId)
        {
            var response = await _reviewRepository.ApproveReview(reviewId);

            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(response);
        }

        [HttpGet("unapproved")]
        public async Task<IActionResult> GetUnapprovedReviews()
        {
            var response = await _reviewRepository.GetUnapprovedReviews();

            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(response);
        }

        [HttpGet("approved/{productId}")]
        public async Task<IActionResult> GetApprovedReviewsByProductId(string productId)
        {
            var response = await _reviewRepository.GetApprovedReviewsByProductId(productId);

            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(response);
        }

        [HttpGet("average-rating/{productId}")]
        public async Task<IActionResult> GetAverageRatingByProductId(string productId)
        {
            var averageRating = await _reviewRepository.GetAverageRatingByProductId(productId);

            return Ok(averageRating);
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            var response = await _reviewRepository.DeleteReview(reviewId);

            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(response);
        }
    }
}
