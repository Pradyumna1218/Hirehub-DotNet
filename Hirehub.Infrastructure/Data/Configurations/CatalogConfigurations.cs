using Hirehub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hirehub.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasIndex(x => x.Name).IsUnique();
    }
}

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FreelancerSkillConfiguration : IEntityTypeConfiguration<FreelancerSkill>
{
    public void Configure(EntityTypeBuilder<FreelancerSkill> builder)
    {
        builder.HasKey(x => new { x.FreelancerProfileId, x.SkillId });

        builder.HasQueryFilter(x => !x.FreelancerProfile.IsDeleted && !x.Skill.IsDeleted);

        builder.HasOne(x => x.FreelancerProfile)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.FreelancerSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}