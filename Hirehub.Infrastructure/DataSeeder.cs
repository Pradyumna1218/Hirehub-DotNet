using Hirehub.Domain.Entities;
using Hirehub.Domain.Enums;
using Hirehub.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hirehub.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await SeedRolesAsync(services);
        await SeedCategoriesAsync(services);
    }

    private static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedCategoriesAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        var desiredCategories = new List<(string Name, string Description)>
    {
        ("Mobile Development", "iOS, Android, and cross-platform apps"),
        ("UI/UX Design", "Interfaces, prototypes, and user research"),
        ("Graphic Design", "Logos, branding, and visual assets"),
        ("Content Writing", "Articles, copywriting, and blogs"),
        ("Digital Marketing", "SEO, social media, and ad campaigns"),
        ("Data & Analytics", "Data analysis, dashboards, and reporting"),
        ("Video Editing", "Editing, motion graphics, and production")
    };

        var existingNames = await db.Categories.Select(c => c.Name).ToListAsync();

        var toAdd = desiredCategories
            .Where(dc => !existingNames.Contains(dc.Name))
            .Select(dc => new Category { Name = dc.Name, Description = dc.Description })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.Categories.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }
}