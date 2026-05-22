namespace Application.DTOs.Dashboard;

public sealed record DashboardStatsDto(
    int TotalEmployees,
    int PendingLeaveRequests,
    int ApprovedLeaveRequests,
    int RejectedLeaveRequests,
    int TotalTenants);
