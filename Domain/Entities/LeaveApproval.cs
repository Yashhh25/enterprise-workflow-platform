using Domain.Enums;

namespace Domain.Entities;

public class LeaveApproval
{
    public Guid Id { get; set; }

    public Guid LeaveRequestId { get; set; }

    public LeaveRequest LeaveRequest { get; set; } = null!;

    public Guid ApprovedByUserId { get; set; }

    public User ApprovedByUser { get; set; } = null!;

    public string Role { get; set; } = string.Empty;

    public LeaveApprovalStatus Status { get; set; }

    public string? Comment { get; set; }

    public DateTime ActionAt { get; set; } = DateTime.UtcNow;
}
