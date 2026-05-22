namespace Application.DTOs.Leave;

public sealed record LeaveRequestResponseDto(
    Guid Id,
    Guid UserId,
    Guid TenantId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason,
    string Status,
    DateTime CreatedAt);
