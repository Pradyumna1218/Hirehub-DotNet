using Hirehub.Application.DTOs.Orders;
using Hirehub.Domain.Entities;

namespace Hirehub.Application.Interfaces;

public interface IOrderService
{
    Task<Order> CreateFromProposalAsync(Proposal proposal);
    Task<List<OrderResponse>> GetMyOrdersAsync(string userId);
    Task<OrderResponse?> GetByIdAsync(int orderId, string userId);
    Task<OrderResponse> UpdateStatusAsync(int orderId, string userId, string newStatus);
}