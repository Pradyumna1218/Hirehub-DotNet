using Hirehub.Domain.Common;

namespace Hirehub.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<ClientProfile> Clients { get; set; } = new List<ClientProfile>();
}