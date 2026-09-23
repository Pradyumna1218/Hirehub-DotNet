using Hirehub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hirehub.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Domain.Entities.Service>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Service> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Price).HasPrecision(18, 2);

        builder.HasIndex(x => x.Title);

        builder.HasOne(x => x.FreelancerProfile)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.Budget).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.ClientProfile)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
{
    public void Configure(EntityTypeBuilder<Proposal> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.CoverLetter).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.ProposedPrice).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

        // One proposal per freelancer per job
        builder.HasIndex(x => new { x.JobId, x.FreelancerProfileId }).IsUnique();

        builder.HasOne(x => x.Job)
            .WithMany(x => x.Proposals)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FreelancerProfile)
            .WithMany(x => x.Proposals)
            .HasForeignKey(x => x.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}