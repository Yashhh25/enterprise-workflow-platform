using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class LeaveApprovalConfiguration : IEntityTypeConfiguration<LeaveApproval>
{
    public void Configure(EntityTypeBuilder<LeaveApproval> builder)
    {
        builder.ToTable("leave_approvals");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.LeaveRequestId)
            .IsRequired();

        builder.Property(a => a.ApprovedByUserId)
            .IsRequired();

        builder.Property(a => a.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Comment)
            .HasMaxLength(1000);

        builder.Property(a => a.ActionAt)
            .IsRequired();

        builder.HasIndex(a => a.LeaveRequestId);

        builder.HasIndex(a => new { a.LeaveRequestId, a.Role })
            .IsUnique();

        builder.HasOne(a => a.LeaveRequest)
            .WithMany(l => l.Approvals)
            .HasForeignKey(a => a.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.ApprovedByUser)
            .WithMany()
            .HasForeignKey(a => a.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
