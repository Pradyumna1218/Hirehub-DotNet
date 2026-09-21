using Hirehub.Domain.Common;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Hirehub.Domain.Entities;

public class FreelancerProfile : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? Location { get; set; }
    public string? PortfolioUrl { get; set; }

    public ICollection<FreelancerSkill> Skills { get; set; } = new List<FreelancerSkill>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}