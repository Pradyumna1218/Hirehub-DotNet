using Hirehub.Api.Extensions;
using Hirehub.Application.DTOs.Jobs;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hirehub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] JobListQuery query)
    {
        var result = await _jobService.GetListAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var job = await _jobService.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = "Job not found." });
        }

        return Ok(job);
    }

    [Authorize(Policy = AppPolicies.CanManageJobs)]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyJobs()
    {
        var jobs = await _jobService.GetMyJobsAsync(User.GetUserId());
        return Ok(jobs);
    }

    [Authorize(Policy = AppPolicies.CanManageJobs)]
    [HttpPost]
    public async Task<IActionResult> Create(JobCreateRequest request)
    {
        try
        {
            var job = await _jobService.CreateAsync(User.GetUserId(), request);
            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}