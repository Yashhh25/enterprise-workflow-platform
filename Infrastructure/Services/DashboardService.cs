using Application.DTOs.Dashboard;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalEmployees = await _dbContext.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var pendingLeaveRequests = await _dbContext.LeaveRequests
            .AsNoTracking()
            .CountAsync(l => l.Status == LeaveRequestStatus.Pending, cancellationToken);

        var approvedLeaveRequests = await _dbContext.LeaveRequests
            .AsNoTracking()
            .CountAsync(l => l.Status == LeaveRequestStatus.Approved, cancellationToken);

        var rejectedLeaveRequests = await _dbContext.LeaveRequests
            .AsNoTracking()
            .CountAsync(l => l.Status == LeaveRequestStatus.Rejected, cancellationToken);

        var totalTenants = await _dbContext.Tenants
            .AsNoTracking()
            .CountAsync(cancellationToken);

        return new DashboardStatsDto(
            totalEmployees,
            pendingLeaveRequests,
            approvedLeaveRequests,
            rejectedLeaveRequests,
            totalTenants);
    }
}
