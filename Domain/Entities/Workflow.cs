using Domain.Enums;

namespace Domain.Entities;

public class Workflow
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public WorkflowStatus Status { get; set; } = WorkflowStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
}
