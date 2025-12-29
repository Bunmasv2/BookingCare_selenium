using server.DTO;
using server.Models;

namespace server.Services.RatingRepository
{
    public interface IReview
    {
        Task<Review> AddReview(ReviewForm reviewForm);
        Task<DoctorReviewDetail> AddDoctorReview(int reviewId, DoctorRatings doctorReview);
        Task<ServiceReviewDetail> AddServiceReview(int reviewId, ServiceRatings serviceReview);
        Task<List<ServiceReview>> GetServiceReviews(int serviceId);
        Task<List<ServiceReview>> GetDoctorReviews(int doctorId);
        Task<List<DoctorReviewDetailDTO>> GetDoctorReviewsDetail(string filter, int doctorId);
        Task<List<ServiceReviewDetailDTO>> GetServiceReviewsDetail(string filter, int serviceId);
        Task<ReviewDTO> CheckExistReview(int recordId);
        Task<List<ReviewRating>> GetDoctorRatings(int doctorId);
        Task<List<ReviewRating>> GetServiceRatings(int serviceId);
        Task<List<ReviewRating>> GetMonthlyRatingReviews(int month, int year, int doctorId);
        Task<List<ReviewRating>> GetMonthlyServiceRatingReviews(int month, int year, int serviceId);
        Task<List<DoctorReviewDetailDTO>> GetDoctorReviewDetail(int doctorId);
    }
}
