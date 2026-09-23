using Hirehub.Application.DTOs.Proposals;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Entities;
using Hirehub.Domain.Enums;
using Hirehub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hirehub.Infrastructure.Services;

public class ProposalService : IProposalService
{
    private readonly ApplicationDbContext _db;

    public ProposalService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ProposalResponse> CreateAsync(string userId, ProposalCreateRequest request)
    {
        var freelancer = await _db.FreelancerProfiles
            .FirstOrDefaultAsync(f => f.UserId == userId)
            ?? throw new InvalidOperationException("Freelancer profile not found for this user.");

        var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == request.JobId)
            ?? throw new InvalidOperationException("Job not found.");

        if (job.Status != JobStatus.Open)
        {
            throw new InvalidOperationException("This job is no longer accepting proposals.");
        }

        var alreadyApplied = await _db.Proposals
            .AnyAsync(p => p.JobId == request.JobId && p.FreelancerProfileId == freelancer.Id);
        if (alreadyApplied)
        {
            throw new InvalidOperationException("You have already submitted a proposal for this job.");
        }

        var proposal = new Proposal
        {
            JobId = request.JobId,
            FreelancerProfileId = freelancer.Id,
            CoverLetter = request.CoverLetter,
            ProposedPrice = request.ProposedPrice,
            DeliveryDays = request.DeliveryDays,
            Status = ProposalStatus.Pending
        };

        _db.Proposals.Add(proposal);
        await _db.SaveChangesAsync();

        return await MapToResponseAsync(proposal.Id)
            ?? throw new InvalidOperationException("Failed to load the created proposal.");
    }

    public async Task<List<ProposalResponse>> GetMyProposalsAsync(string userId)
    {
        var freelancer = await _db.FreelancerProfiles.FirstOrDefaultAsync(f => f.UserId == userId);
        if (freelancer == null)
        {
            return new List<ProposalResponse>();
        }

        return await _db.Proposals
            .Include(p => p.Job)
            .Include(p => p.FreelancerProfile)
            .Where(p => p.FreelancerProfileId == freelancer.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToResponse(p))
            .ToListAsync();
    }

    public async Task<List<ProposalResponse>> GetForJobAsync(int jobId, string clientUserId)
    {
        await EnsureClientOwnsJobAsync(jobId, clientUserId);

        return await _db.Proposals
            .Include(p => p.Job)
            .Include(p => p.FreelancerProfile)
            .Where(p => p.JobId == jobId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToResponse(p))
            .ToListAsync();
    }

    public async Task<ProposalResponse> AcceptAsync(int proposalId, string clientUserId)
    {
        var proposal = await _db.Proposals
            .Include(p => p.Job)
            .FirstOrDefaultAsync(p => p.Id == proposalId)
            ?? throw new InvalidOperationException("Proposal not found.");

        await EnsureClientOwnsJobAsync(proposal.JobId, clientUserId);

        if (proposal.Status != ProposalStatus.Pending)
        {
            throw new InvalidOperationException("Only pending proposals can be accepted.");
        }

        proposal.Status = ProposalStatus.Accepted;
        proposal.Job.Status = JobStatus.InProgress;

        var otherPending = await _db.Proposals
            .Where(p => p.JobId == proposal.JobId && p.Id != proposal.Id && p.Status == ProposalStatus.Pending)
            .ToListAsync();

        foreach (var other in otherPending)
        {
            other.Status = ProposalStatus.Rejected;
        }

        await _db.SaveChangesAsync();

        return await MapToResponseAsync(proposal.Id)
            ?? throw new InvalidOperationException("Failed to load the accepted proposal.");
    }

    public async Task RejectAsync(int proposalId, string clientUserId)
    {
        var proposal = await _db.Proposals
            .FirstOrDefaultAsync(p => p.Id == proposalId)
            ?? throw new InvalidOperationException("Proposal not found.");

        await EnsureClientOwnsJobAsync(proposal.JobId, clientUserId);

        if (proposal.Status != ProposalStatus.Pending)
        {
            throw new InvalidOperationException("Only pending proposals can be rejected.");
        }

        proposal.Status = ProposalStatus.Rejected;
        await _db.SaveChangesAsync();
    }

    private async Task EnsureClientOwnsJobAsync(int jobId, string clientUserId)
    {
        var job = await _db.Jobs
            .Include(j => j.ClientProfile)
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new InvalidOperationException("Job not found.");

        if (job.ClientProfile.UserId != clientUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to manage proposals for this job.");
        }
    }

    private async Task<ProposalResponse?> MapToResponseAsync(int proposalId)
    {
        var proposal = await _db.Proposals
            .Include(p => p.Job)
            .Include(p => p.FreelancerProfile)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        return proposal == null ? null : MapToResponse(proposal);
    }

    private static ProposalResponse MapToResponse(Proposal p) => new()
    {
        Id = p.Id,
        JobId = p.JobId,
        JobTitle = p.Job.Title,
        CoverLetter = p.CoverLetter,
        ProposedPrice = p.ProposedPrice,
        DeliveryDays = p.DeliveryDays,
        Status = p.Status.ToString(),
        FreelancerName = p.FreelancerProfile.FullName,
        CreatedAt = p.CreatedAt
    };
}