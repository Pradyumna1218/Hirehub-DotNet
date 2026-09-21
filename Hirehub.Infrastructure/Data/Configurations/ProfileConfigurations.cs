using Hirehub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hirehub.Infrastructure.Data.Configurations;

public class FreelancerProfileConfiguration : IEntityTypeConfiguration<FreelancerProfile>
{
    public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Bio).HasMaxLength(2000);
        builder.Property(x => x.Location).HasMaxLength(150);
        builder.Property(x => x.PortfolioUrl).HasMaxLength(300);
        builder.Property(x => x.HourlyRate).HasPrecision(18, 2);

        builder.HasIndex(x => x.UserId).IsUnique();
    }
}

public class ClientProfileConfiguration : IEntityTypeConfiguration<ClientProfile>
{
    public void Configure(EntityTypeBuilder<ClientProfile> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.CompanyName).HasMaxLength(150);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasMany(x => x.PreferredCategories)
            .WithMany(x => x.Clients)
            .UsingEntity(j => j.ToTable("ClientPreferredCategories"));
    }
}