using Hirehub.Application.DTOs.Common;
using Hirehub.Application.DTOs.Jobs;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Entities;
using Hirehub.Domain.Enums;
using Hirehub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hirehub.Infrastructure.Service;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _db;

    public JobService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<JobResponse> CreateAsync(string userId, JobCreateRequest request)
    {
        var clientProfile = await _db.ClientProfiles
            .FirstOrDefaultAsync(c => c.UserId == userId)
            ?? throw new InvalidOperationException("Client profile not found for this user.");

        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            throw new InvalidOperationException("Selected category does not exist.");
        }

        var job = new Job
        {
            Title = request.Title,
            Description = request.Description,
            Budget = request.Budget,
            Deadline = request.Deadline,
            CategoryId = request.CategoryId,
            ClientProfileId = clientProfile.Id,
            Status = JobStatus.Open
        };

        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();

        return await MapToResponseAsync(job.Id)
            ?? throw new InvalidOperationException("Failed to load the created job.");
    }

    public async Task<PagedResult<JobResponse>> GetListAsync(JobListQuery query)
    {
        var jobsQuery = _db.Jobs
            .Include(j => j.Category)
            .Include(j => j.ClientProfile)
            .Include(j => j.Proposals)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            jobsQuery = jobsQuery.Where(j => j.Title.Contains(query.Search));
        }

        if (query.CategoryId.HasValue)
        {
            jobsQuery = jobsQuery.Where(j => j.CategoryId == query.CategoryId.Value);
        }

        if (query.MinBudget.HasValue)
        {
            jobsQuery = jobsQuery.Where(j => j.Budget >= query.MinBudget.Value);
        }

        if (query.MaxBudget.HasValue)
        {
            jobsQuery = jobsQuery.Where(j => j.Budget <= query.MaxBudget.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<JobStatus>(query.Status, true, out var status))
        {
            jobsQuery = jobsQuery.Where(j => j.Status == status);
        }

        var totalCount = await jobsQuery.CountAsync();

        var pageSize = query.PageSize is > 0 and <= 50 ? query.PageSize : 10;
        var pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;

        var jobs = await jobsQuery
            .OrderByDescending(j => j.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(j => MapToResponse(j))
            .ToListAsync();

        return new PagedResult<JobResponse>
        {
            Items = jobs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<JobResponse?> GetByIdAsync(int id)
    {
        return await MapToResponseAsync(id);
    }

    public async Task<List<JobResponse>> GetMyJobsAsync(string userId)
    {
        var clientProfile = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);
        if (clientProfile == null)
        {
            return new List<JobResponse>();
        }

        return await _db.Jobs
            .Include(j => j.Category)
            .Include(j => j.ClientProfile)
            .Include(j => j.Proposals)
            .Where(j => j.ClientProfileId == clientProfile.Id)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => MapToResponse(j))
            .ToListAsync();
    }

    private async Task<JobResponse?> MapToResponseAsync(int jobId)
    {
        var job = await _db.Jobs
            .Include(j => j.Category)
            .Include(j => j.ClientProfile)
            .Include(j => j.Proposals)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        return job == null ? null : MapToResponse(job);
    }

    private static JobResponse MapToResponse(Job job) => new()
    {
        Id = job.Id,
        Title = job.Title,
        Description = job.Description,
        Budget = job.Budget,
        Deadline = job.Deadline,
        Status = job.Status.ToString(),
        CategoryName = job.Category.Name,
        ClientName = job.ClientProfile.FullName,
        ProposalCount = job.Proposals.Count,
        CreatedAt = job.CreatedAt
    };
}