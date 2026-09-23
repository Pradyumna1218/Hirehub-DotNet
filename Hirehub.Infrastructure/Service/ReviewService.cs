using Hirehub.Application.DTOs.Reviews;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Entities;
using Hirehub.Domain.Enums;
using Hirehub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hirehub.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _db;

    public ReviewService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewResponse> CreateAsync(string clientUserId, ReviewCreateRequest request)
    {
        var order = await _db.Orders
            .Include(o => o.ClientProfile)
            .Include(o => o.FreelancerProfile)
            .Include(o => o.Review)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId)
            ?? throw new InvalidOperationException("Order not found.");

        if (order.ClientProfile.UserId != clientUserId)
        {
            throw new UnauthorizedAccessException("You can only review your own orders.");
        }

        if (order.Status != OrderStatus.Completed)
        {
            throw new InvalidOperationException("Only completed orders can be reviewed.");
        }

        if (order.Review != null)
        {
            throw new InvalidOperationException("This order has already been reviewed.");
        }

        var review = new Review
        {
            OrderId = order.Id,
            ClientProfileId = order.ClientProfileId,
            FreelancerProfileId = order.FreelancerProfileId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return new ReviewResponse
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            ClientName = order.ClientProfile.FullName,
            FreelancerName = order.FreelancerProfile.FullName,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<List<ReviewResponse>> GetForFreelancerAsync(int freelancerProfileId)
    {
        return await _db.Reviews
            .Include(r => r.ClientProfile)
            .Include(r => r.FreelancerProfile)
            .Where(r => r.FreelancerProfileId == freelancerProfileId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                ClientName = r.ClientProfile.FullName,
                FreelancerName = r.FreelancerProfile.FullName,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<FreelancerRatingResponse> GetRatingAsync(int freelancerProfileId)
    {
        var ratings = await _db.Reviews
            .Where(r => r.FreelancerProfileId == freelancerProfileId)
            .Select(r => r.Rating)
            .ToListAsync();

        if (ratings.Count == 0)
        {
            return new FreelancerRatingResponse { AverageRating = 0, ReviewCount = 0 };
        }

        return new FreelancerRatingResponse
        {
            AverageRating = Math.Round((decimal)ratings.Average(), 1),
            ReviewCount = ratings.Count
        };
    }
}