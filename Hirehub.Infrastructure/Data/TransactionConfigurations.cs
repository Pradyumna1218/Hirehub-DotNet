using Hirehub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hirehub.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.ClientProfile)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FreelancerProfile)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Proposal)
            .WithOne(x => x.Order)
            .HasForeignKey<Order>(x => x.ProposalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Provider).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ProviderToken).HasMaxLength(255);
        builder.Property(x => x.ProviderTransactionId).HasMaxLength(255);
        builder.Property(x => x.PayerUserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => x.ProviderTransactionId);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Comment).HasMaxLength(1000);

        // Rating must be 1 to 5
        builder.ToTable(t => t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5"));

        builder.HasOne(x => x.Order)
            .WithOne(x => x.Review)
            .HasForeignKey<Review>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClientProfile)
            .WithMany()
            .HasForeignKey(x => x.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FreelancerProfile)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(500);

        builder.HasIndex(x => new { x.UserId, x.IsRead });
    }
}