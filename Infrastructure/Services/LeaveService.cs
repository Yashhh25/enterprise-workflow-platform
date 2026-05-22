using Application.DTOs.Leave;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Audit;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class LeaveService : ILeaveService
{
    private const string ManagerRole = "Manager";
    private const string HrRole = "HR";
    private const string EmployeeRole = "Employee";

    private readonly ApplicationDbContext _dbContext;
    private readonly IAuditService _auditService;

    public LeaveService(ApplicationDbContext dbContext, IAuditService auditService)
    {
        _dbContext = dbContext;
        _auditService = auditService;
    }

    public async Task<LeaveRequestResponseDto> CreateLeaveRequestAsync(
        CreateLeaveRequestDto request,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserBelongsToTenantAsync(userId, tenantId, cancellationToken);

        if (request.StartDate > request.EndDate)
        {
            throw new ArgumentException("Start date must be on or before end date.");
        }

        var leaveRequest = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = tenantId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason.Trim(),
            Status = LeaveRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.LeaveRequests.Add(leaveRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            AuditEntities.LeaveRequest,
            AuditActions.Created,
            userId,
            tenantId,
            $"Leave request {leaveRequest.Id} created for {leaveRequest.StartDate:yyyy-MM-dd} to {leaveRequest.EndDate:yyyy-MM-dd}.",
            cancellationToken);

        return MapToResponse(leaveRequest);
    }

    public async Task<IReadOnlyList<LeaveRequestResponseDto>> GetMyRequestsAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserBelongsToTenantAsync(userId, tenantId, cancellationToken);

        var requests = await _dbContext.LeaveRequests
            .AsNoTracking()
            .Where(l => l.UserId == userId && l.TenantId == tenantId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        return requests.Select(MapToResponse).ToList();
    }

    public async Task<LeaveRequestResponseDto> ApproveLeaveRequestAsync(
        Guid leaveRequestId,
        Guid userId,
        Guid tenantId,
        string role,
        LeaveActionDto? request,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserBelongsToTenantAsync(userId, tenantId, cancellationToken);
        EnsureApproverRole(role);

        var leaveRequest = await GetLeaveRequestForActionAsync(leaveRequestId, tenantId, cancellationToken);
        EnsureRequestIsActionable(leaveRequest);
        EnsureNotSelfApproval(leaveRequest, userId);

        if (role == ManagerRole)
        {
            EnsureNoExistingApproval(leaveRequest, ManagerRole);

            leaveRequest.Approvals.Add(CreateApproval(
                leaveRequest.Id,
                userId,
                ManagerRole,
                LeaveApprovalStatus.Approved,
                request?.Comment));

            leaveRequest.Status = LeaveRequestStatus.Pending;
        }
        else if (role == HrRole)
        {
            EnsureManagerApproved(leaveRequest);
            EnsureNoExistingApproval(leaveRequest, HrRole);

            leaveRequest.Approvals.Add(CreateApproval(
                leaveRequest.Id,
                userId,
                HrRole,
                LeaveApprovalStatus.Approved,
                request?.Comment));

            leaveRequest.Status = LeaveRequestStatus.Approved;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            AuditEntities.LeaveRequest,
            AuditActions.Approved,
            userId,
            tenantId,
            $"Leave request {leaveRequest.Id} approved by {role}. Status: {leaveRequest.Status}.",
            cancellationToken);

        return MapToResponse(leaveRequest);
    }

    public async Task<LeaveRequestResponseDto> RejectLeaveRequestAsync(
        Guid leaveRequestId,
        Guid userId,
        Guid tenantId,
        string role,
        LeaveActionDto? request,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserBelongsToTenantAsync(userId, tenantId, cancellationToken);
        EnsureApproverRole(role);

        var leaveRequest = await GetLeaveRequestForActionAsync(leaveRequestId, tenantId, cancellationToken);
        EnsureRequestIsActionable(leaveRequest);
        EnsureNotSelfApproval(leaveRequest, userId);

        if (role == ManagerRole)
        {
            EnsureNoExistingApproval(leaveRequest, ManagerRole);

            leaveRequest.Approvals.Add(CreateApproval(
                leaveRequest.Id,
                userId,
                ManagerRole,
                LeaveApprovalStatus.Rejected,
                request?.Comment));
        }
        else if (role == HrRole)
        {
            EnsureManagerApproved(leaveRequest);
            EnsureNoExistingApproval(leaveRequest, HrRole);

            leaveRequest.Approvals.Add(CreateApproval(
                leaveRequest.Id,
                userId,
                HrRole,
                LeaveApprovalStatus.Rejected,
                request?.Comment));
        }

        leaveRequest.Status = LeaveRequestStatus.Rejected;
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            AuditEntities.LeaveRequest,
            AuditActions.Rejected,
            userId,
            tenantId,
            $"Leave request {leaveRequest.Id} rejected by {role}.",
            cancellationToken);

        return MapToResponse(leaveRequest);
    }

    private async Task<LeaveRequest> GetLeaveRequestForActionAsync(
        Guid leaveRequestId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await _dbContext.LeaveRequests
            .Include(l => l.Approvals)
            .FirstOrDefaultAsync(l => l.Id == leaveRequestId && l.TenantId == tenantId, cancellationToken);

        if (leaveRequest is null)
        {
            throw new KeyNotFoundException("Leave request was not found.");
        }

        return leaveRequest;
    }

    private static void EnsureApproverRole(string role)
    {
        if (role is not (ManagerRole or HrRole))
        {
            throw new UnauthorizedAccessException("Only Manager or HR can perform this action.");
        }
    }

    private static void EnsureRequestIsActionable(LeaveRequest leaveRequest)
    {
        if (leaveRequest.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException("Leave request is no longer pending approval.");
        }
    }

    private static void EnsureNotSelfApproval(LeaveRequest leaveRequest, Guid userId)
    {
        if (leaveRequest.UserId == userId)
        {
            throw new InvalidOperationException("You cannot approve or reject your own leave request.");
        }
    }

    private static void EnsureNoExistingApproval(LeaveRequest leaveRequest, string role)
    {
        if (leaveRequest.Approvals.Any(a => a.Role == role))
        {
            throw new InvalidOperationException($"A {role} action has already been recorded for this leave request.");
        }
    }

    private static void EnsureManagerApproved(LeaveRequest leaveRequest)
    {
        var managerApproved = leaveRequest.Approvals.Any(a =>
            a.Role == ManagerRole && a.Status == LeaveApprovalStatus.Approved);

        if (!managerApproved)
        {
            throw new InvalidOperationException("Manager approval is required before HR can act on this request.");
        }
    }

    private static LeaveApproval CreateApproval(
        Guid leaveRequestId,
        Guid userId,
        string role,
        LeaveApprovalStatus status,
        string? comment) =>
        new()
        {
            Id = Guid.NewGuid(),
            LeaveRequestId = leaveRequestId,
            ApprovedByUserId = userId,
            Role = role,
            Status = status,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
            ActionAt = DateTime.UtcNow
        };

    private async Task EnsureUserBelongsToTenantAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && u.TenantId == tenantId, cancellationToken);

        if (!userExists)
        {
            throw new UnauthorizedAccessException("User is not authorized for this tenant.");
        }
    }

    private static LeaveRequestResponseDto MapToResponse(LeaveRequest leaveRequest) =>
        new(
            leaveRequest.Id,
            leaveRequest.UserId,
            leaveRequest.TenantId,
            leaveRequest.StartDate,
            leaveRequest.EndDate,
            leaveRequest.Reason,
            leaveRequest.Status.ToString(),
            leaveRequest.CreatedAt);
}
