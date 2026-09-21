using Microsoft.AspNetCore.Identity;

namespace Hirehub.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}