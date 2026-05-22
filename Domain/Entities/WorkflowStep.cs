using Domain.Enums;

namespace Domain.Entities;

public class WorkflowStep
{
    public Guid Id { get; set; }

    public Guid WorkflowId { get; set; }

    public Workflow Workflow { get; set; } = null!;

    public int StepOrder { get; set; }

    public string AssignedRole { get; set; } = string.Empty;

    public WorkflowStepStatus Status { get; set; } = WorkflowStepStatus.Pending;

    public Guid? ApprovedByUserId { get; set; }

    public User? ApprovedByUser { get; set; }

    public DateTime? ApprovedAt { get; set; }
}
