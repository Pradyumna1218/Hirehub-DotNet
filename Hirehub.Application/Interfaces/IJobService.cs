using Hirehub.Application.DTOs.Common;
using Hirehub.Application.DTOs.Jobs;

namespace Hirehub.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateAsync(string userId, JobCreateRequest request);
    Task<PagedResult<JobResponse>> GetListAsync(JobListQuery query);
    Task<JobResponse?> GetByIdAsync(int id);
    Task<List<JobResponse>> GetMyJobsAsync(string userId);
}