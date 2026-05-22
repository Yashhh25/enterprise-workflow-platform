using Domain.Enums;

namespace Domain.Entities;

public class LeaveRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LeaveApproval> Approvals { get; set; } = new List<LeaveApproval>();
}
