using Hirehub.Api.Extensions;
using Hirehub.Application.DTOs.Proposals;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hirehub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProposalsController : ControllerBase
{
    private readonly IProposalService _proposalService;

    public ProposalsController(IProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    [Authorize(Policy = AppPolicies.CanSubmitProposals)]
    [HttpPost]
    public async Task<IActionResult> Create(ProposalCreateRequest request)
    {
        try
        {
            var proposal = await _proposalService.CreateAsync(User.GetUserId(), request);
            return Ok(proposal);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = AppPolicies.CanSubmitProposals)]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyProposals()
    {
        var proposals = await _proposalService.GetMyProposalsAsync(User.GetUserId());
        return Ok(proposals);
    }

    [Authorize(Policy = AppPolicies.CanManageJobs)]
    [HttpGet("job/{jobId:int}")]
    public async Task<IActionResult> GetForJob(int jobId)
    {
        try
        {
            var proposals = await _proposalService.GetForJobAsync(jobId, User.GetUserId());
            return Ok(proposals);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Policy = AppPolicies.CanManageJobs)]
    [HttpPost("{id:int}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        try
        {
            var proposal = await _proposalService.AcceptAsync(id, User.GetUserId());
            return Ok(proposal);
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

    [Authorize(Policy = AppPolicies.CanManageJobs)]
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            await _proposalService.RejectAsync(id, User.GetUserId());
            return NoContent();
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
}