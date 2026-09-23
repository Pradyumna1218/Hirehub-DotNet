using Hirehub.Application.DTOs.Orders;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Entities;
using Hirehub.Domain.Enums;
using Hirehub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hirehub.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _db;

    public OrderService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Order> CreateFromProposalAsync(Proposal proposal)
    {
        var job = await _db.Jobs.FirstAsync(j => j.Id == proposal.JobId);

        var order = new Order
        {
            ClientProfileId = job.ClientProfileId,
            FreelancerProfileId = proposal.FreelancerProfileId,
            ProposalId = proposal.Id,
            TotalAmount = proposal.ProposedPrice,
            DeliveryDate = DateTime.UtcNow.AddDays(proposal.DeliveryDays),
            Status = OrderStatus.Pending
        };

        _db.Orders.Add(order);
        return order;
    }

    public async Task<List<OrderResponse>> GetMyOrdersAsync(string userId)
    {
        var orders = await _db.Orders
            .Include(o => o.ClientProfile)
            .Include(o => o.FreelancerProfile)
            .Include(o => o.Proposal).ThenInclude(p => p!.Job)
            .Include(o => o.Service)
            .Where(o => o.ClientProfile.UserId == userId || o.FreelancerProfile.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(int orderId, string userId)
    {
        var order = await _db.Orders
            .Include(o => o.ClientProfile)
            .Include(o => o.FreelancerProfile)
            .Include(o => o.Proposal).ThenInclude(p => p!.Job)
            .Include(o => o.Service)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return null;
        }

        if (order.ClientProfile.UserId != userId && order.FreelancerProfile.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this order.");
        }

        return MapToResponse(order);
    }

    public async Task<OrderResponse> UpdateStatusAsync(int orderId, string userId, string newStatus)
    {
        var order = await _db.Orders
            .Include(o => o.ClientProfile)
            .Include(o => o.FreelancerProfile)
            .Include(o => o.Proposal).ThenInclude(p => p!.Job)
            .Include(o => o.Service)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new InvalidOperationException("Order not found.");

        var isClient = order.ClientProfile.UserId == userId;
        var isFreelancer = order.FreelancerProfile.UserId == userId;

        if (!isClient && !isFreelancer)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this order.");
        }

        if (!Enum.TryParse<OrderStatus>(newStatus, true, out var target))
        {
            throw new InvalidOperationException("Invalid order status.");
        }

        ValidateTransition(order.Status, target, isClient, isFreelancer);

        order.Status = target;

        if (target == OrderStatus.Completed && order.Proposal != null)
        {
            order.Proposal.Job.Status = JobStatus.Completed;
        }

        await _db.SaveChangesAsync();

        return MapToResponse(order);
    }

    private static void ValidateTransition(OrderStatus current, OrderStatus target, bool isClient, bool isFreelancer)
    {
        var allowed = (current, target) switch
        {
            (OrderStatus.Pending, OrderStatus.InProgress) when isFreelancer => true,
            (OrderStatus.InProgress, OrderStatus.Delivered) when isFreelancer => true,
            (OrderStatus.Delivered, OrderStatus.Completed) when isClient => true,
            (OrderStatus.Delivered, OrderStatus.InProgress) when isClient => true, // client requests revision
            (_, OrderStatus.Cancelled) when isClient || isFreelancer => current is OrderStatus.Pending or OrderStatus.InProgress,
            _ => false
        };

        if (!allowed)
        {
            throw new InvalidOperationException(
                $"Cannot move an order from '{current}' to '{target}' as {(isClient ? "the client" : "the freelancer")}.");
        }
    }

    private static OrderResponse MapToResponse(Order o) => new()
    {
        Id = o.Id,
        TotalAmount = o.TotalAmount,
        DeliveryDate = o.DeliveryDate,
        Status = o.Status.ToString(),
        ClientName = o.ClientProfile.FullName,
        FreelancerName = o.FreelancerProfile.FullName,
        JobTitle = o.Proposal?.Job.Title,
        ServiceTitle = o.Service?.Title,
        CreatedAt = o.CreatedAt
    };
}