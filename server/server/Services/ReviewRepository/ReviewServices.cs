using AutoMapper;
using Microsoft.EntityFrameworkCore;
using server.DTO;
using server.Models;

namespace server.Services.RatingRepository
{
    public class ReviewServices : IReview
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;

        public ReviewServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ReviewDTO> CheckExistReview(int recordId)
        {
            var review = await _context.Reviews
                .Include(review => review.DoctorReviewDetail)
                .Include(review => review.ServiceReviewDetail)
                .FirstOrDefaultAsync(review => review.PrescriptionId == recordId);

            var reviewDTO = _mapper.Map<ReviewDTO>(review);

            return reviewDTO;
        }
        public async Task<Review> AddReview(ReviewForm reviewForm)
        {
            Review review = new Review
            {
                PrescriptionId = reviewForm.RecordId,
                OverallRating = reviewForm.OverallRating,
                Comment = reviewForm.Comment,
                CreatedAt = DateTime.Now
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<DoctorReviewDetail> AddDoctorReview(int reviewId, DoctorRatings doctorReview)
        {
            DoctorReviewDetail doctorReviewDetail = new DoctorReviewDetail
            {
                ReviewId = reviewId,
                Knowledge = doctorReview.Knowledge,
                Attitude = doctorReview.Attitude,
                Dedication = doctorReview.Dedication,
                CommunicationSkill = doctorReview.CommunicationSkill
            };

            await _context.DoctorReviewDetails.AddAsync(doctorReviewDetail);
            await _context.SaveChangesAsync();
            return doctorReviewDetail;
        }

        public async Task<ServiceReviewDetail> AddServiceReview(int reviewId, ServiceRatings serviceReview)
        {
            ServiceReviewDetail serviceReviewDetail = new ServiceReviewDetail
            {
                ReviewId = reviewId,
                Effectiveness = serviceReview.Effectiveness,
                Price = serviceReview.Price,
                ServiceSpeed = serviceReview.ServiceSpeed,
                Convenience = serviceReview.Convenience
            };

            await _context.ServiceReviewDetails.AddAsync(serviceReviewDetail);
            await _context.SaveChangesAsync();
            return serviceReviewDetail;
        }

        public async Task<List<ServiceReview>> GetServiceReviews(int serviceId)
        {
            var serviceReviews = await _context.Reviews
                .Include(review => review.MedicalRecord.Appointment.Patient.User)
                .Include(review => review.MedicalRecord.Appointment.Service)
                .Where(r => r.MedicalRecord.Appointment.Service.ServiceId == serviceId)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            var serviceReviewDTOs = _mapper.Map<List<ServiceReview>>(serviceReviews);

            return serviceReviewDTOs;
        }

        public async Task<List<ServiceReview>> GetDoctorReviews(int doctorId)
        {
            var serviceReviews = await _context.Reviews
                .Include(review => review.MedicalRecord.Appointment.Patient.User)
                .Include(review => review.MedicalRecord.Appointment.Doctor)
                .Where(r => r.MedicalRecord.Appointment.Doctor.DoctorId == doctorId)
                .Take(3)
                .ToListAsync();

            var serviceReviewDTOs = _mapper.Map<List<ServiceReview>>(serviceReviews);

            return serviceReviewDTOs;
        }

        public async Task<List<DoctorReviewDetailDTO>> GetDoctorReviewsDetail(string filter, int doctorId)
        {
            var query = _context.Reviews
                .Include(review => review.MedicalRecord)
                .Include(review => review.DoctorReviewDetail)
                .Include(review => review.MedicalRecord.Appointment)
                .Include(review => review.MedicalRecord.Appointment.Patient)
                .Include(review => review.MedicalRecord.Appointment.Doctor)
                .Include(review => review.MedicalRecord.Appointment.Patient.User)
                .Include(review => review.MedicalRecord.Appointment.Service)
                .Where(review => review.MedicalRecord.Appointment.Doctor.DoctorId == doctorId);

            if (filter == "positive")
            {
                query = query.Where(review => review.OverallRating > 3);
            }
            else if (filter == "negative")
            {
                query = query.Where(review => review.OverallRating <= 3);
            }

            var reviews = await query.ToListAsync();

            var reviewDTOs = _mapper.Map<List<DoctorReviewDetailDTO>>(reviews);

            return reviewDTOs;
        }

        public async Task<List<ServiceReviewDetailDTO>> GetServiceReviewsDetail(string filter, int serviceId)
        {
            var query = _context.Reviews
                .Include(review => review.MedicalRecord)
                .Include(review => review.ServiceReviewDetail)
                .Include(review => review.MedicalRecord.Appointment)
                .Include(review => review.MedicalRecord.Appointment.Patient)
                .Include(review => review.MedicalRecord.Appointment.Doctor)
                .Include(review => review.MedicalRecord.Appointment.Doctor.User)
                .Include(review => review.MedicalRecord.Appointment.Service)
                .Include(review => review.MedicalRecord.Appointment.Patient.User)
                .Where(review => review.MedicalRecord.Appointment.Service.ServiceId == serviceId);

            if (filter == "positive")
            {
                query = query.Where(review => review.OverallRating > 3);
            }
            else if (filter == "negative")
            {
                query = query.Where(review => review.OverallRating <= 3);
            }

            var reviews = await query.ToListAsync();

            var reviewDTOs = _mapper.Map<List<ServiceReviewDetailDTO>>(reviews);

            return reviewDTOs;
        }

        public async Task<List<ReviewRating>> GetDoctorRatings(int doctorId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.MedicalRecord)
                .Include(r => r.MedicalRecord.Appointment)
                .Include(r => r.MedicalRecord.Appointment.Doctor)
                .Where(r => r.MedicalRecord.Appointment.Doctor.DoctorId == doctorId)
                .GroupBy(r => r.OverallRating)
                .Select(group => new ReviewRating
                {
                    Rating = group.Key,
                    ReviewCount = group.Count()
                })
                .ToListAsync();

            var result = Enumerable.Range(1, 5)
                .Select(rating => new ReviewRating
                {
                    Rating = rating,
                    ReviewCount = reviews.FirstOrDefault(x => x.Rating == rating)?.ReviewCount ?? 0
                })
                .ToList();

            return result;
        }

        public async Task<List<ReviewRating>> GetServiceRatings(int serviceId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.MedicalRecord)
                .Include(r => r.MedicalRecord.Appointment)
                .Include(r => r.MedicalRecord.Appointment.Service)
                .Where(r => r.MedicalRecord.Appointment.Service.ServiceId == serviceId)
                .GroupBy(r => r.OverallRating)
                .Select(group => new ReviewRating
                {
                    Rating = group.Key,
                    ReviewCount = group.Count()
                })
                .ToListAsync();

            var result = Enumerable.Range(1, 5)
                .Select(rating => new ReviewRating
                {
                    Rating = rating,
                    ReviewCount = reviews.FirstOrDefault(x => x.Rating == rating)?.ReviewCount ?? 0
                })
                .ToList();

            return result;
        }
        public async Task<List<ReviewRating>> GetMonthlyRatingReviews(int month, int year, int doctorId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.MedicalRecord)
                .Include(r => r.MedicalRecord.Appointment)
                .Include(r => r.MedicalRecord.Appointment.Doctor)
                .Where(r =>
                    r.MedicalRecord.Appointment.Doctor.DoctorId == doctorId
                    && r.MedicalRecord.Appointment.AppointmentDate.Value.Month == month
                    && r.MedicalRecord.Appointment.AppointmentDate.Value.Year == year
                )
                .GroupBy(r => r.OverallRating)
                .Select(group => new ReviewRating
                {
                    Rating = group.Key,
                    ReviewCount = group.Count()
                })
                .ToListAsync();

            var result = Enumerable.Range(1, 5)
                .Select(rating => new ReviewRating
                {
                    Rating = rating,
                    ReviewCount = reviews.FirstOrDefault(x => x.Rating == rating)?.ReviewCount ?? 0
                })
                .ToList();

            return result;
        }

        public async Task<List<ReviewRating>> GetMonthlyServiceRatingReviews(int month, int year, int serviceId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.MedicalRecord)
                .Include(r => r.MedicalRecord.Appointment)
                .Include(r => r.MedicalRecord.Appointment.Service)
                .Where(r =>
                    r.MedicalRecord.Appointment.Service.ServiceId == serviceId
                    && r.MedicalRecord.Appointment.AppointmentDate.Value.Month == month
                    && r.MedicalRecord.Appointment.AppointmentDate.Value.Year == year
                )
                .GroupBy(r => r.OverallRating)
                .Select(group => new ReviewRating
                {
                    Rating = group.Key,
                    ReviewCount = group.Count()
                })
                .ToListAsync();

            var result = Enumerable.Range(1, 5)
                .Select(rating => new ReviewRating
                {
                    Rating = rating,
                    ReviewCount = reviews.FirstOrDefault(x => x.Rating == rating)?.ReviewCount ?? 0
                })
                .ToList();

            return result;
        }

        public async Task<List<DoctorReviewDetailDTO>> GetDoctorReviewDetail(int doctorId)
        {
            return null;
        }
    }
}