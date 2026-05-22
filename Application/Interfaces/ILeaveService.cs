using Application.DTOs.Leave;

namespace Application.Interfaces;

public interface ILeaveService
{
    Task<LeaveRequestResponseDto> CreateLeaveRequestAsync(
        CreateLeaveRequestDto request,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequestResponseDto>> GetMyRequestsAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<LeaveRequestResponseDto> ApproveLeaveRequestAsync(
        Guid leaveRequestId,
        Guid userId,
        Guid tenantId,
        string role,
        LeaveActionDto? request,
        CancellationToken cancellationToken = default);

    Task<LeaveRequestResponseDto> RejectLeaveRequestAsync(
        Guid leaveRequestId,
        Guid userId,
        Guid tenantId,
        string role,
        LeaveActionDto? request,
        CancellationToken cancellationToken = default);
}
