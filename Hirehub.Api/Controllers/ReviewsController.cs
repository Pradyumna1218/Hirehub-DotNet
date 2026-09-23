using Hirehub.Api.Extensions;
using Hirehub.Application.DTOs.Reviews;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hirehub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [Authorize(Policy = AppPolicies.CanLeaveReviews)]
    [HttpPost]
    public async Task<IActionResult> Create(ReviewCreateRequest request)
    {
        try
        {
            var review = await _reviewService.CreateAsync(User.GetUserId(), request);
            return Ok(review);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("freelancer/{freelancerProfileId:int}")]
    public async Task<IActionResult> GetForFreelancer(int freelancerProfileId)
    {
        var reviews = await _reviewService.GetForFreelancerAsync(freelancerProfileId);
        return Ok(reviews);
    }

    [HttpGet("freelancer/{freelancerProfileId:int}/rating")]
    public async Task<IActionResult> GetRating(int freelancerProfileId)
    {
        var rating = await _reviewService.GetRatingAsync(freelancerProfileId);
        return Ok(rating);
    }
}