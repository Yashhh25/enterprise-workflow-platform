using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
{
    public void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.ToTable("workflow_steps");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.WorkflowId)
            .IsRequired();

        builder.Property(s => s.StepOrder)
            .IsRequired();

        builder.Property(s => s.AssignedRole)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(s => s.WorkflowId);

        builder.HasIndex(s => new { s.WorkflowId, s.StepOrder })
            .IsUnique();

        builder.HasIndex(s => s.ApprovedByUserId);

        builder.HasOne(s => s.ApprovedByUser)
            .WithMany()
            .HasForeignKey(s => s.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
