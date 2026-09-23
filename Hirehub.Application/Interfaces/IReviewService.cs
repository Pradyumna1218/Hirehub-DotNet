using Hirehub.Application.DTOs.Reviews;

namespace Hirehub.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewResponse> CreateAsync(string clientUserId, ReviewCreateRequest request);
    Task<List<ReviewResponse>> GetForFreelancerAsync(int freelancerProfileId);
    Task<FreelancerRatingResponse> GetRatingAsync(int freelancerProfileId);
}