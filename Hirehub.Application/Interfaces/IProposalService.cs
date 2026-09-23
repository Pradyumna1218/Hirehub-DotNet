using Hirehub.Application.DTOs.Proposals;

namespace Hirehub.Application.Interfaces;

public interface IProposalService
{
    Task<ProposalResponse> CreateAsync(string userId, ProposalCreateRequest request);
    Task<List<ProposalResponse>> GetMyProposalsAsync(string userId);
    Task<List<ProposalResponse>> GetForJobAsync(int jobId, string clientUserId);
    Task<ProposalResponse> AcceptAsync(int proposalId, string clientUserId);
    Task RejectAsync(int proposalId, string clientUserId);
}