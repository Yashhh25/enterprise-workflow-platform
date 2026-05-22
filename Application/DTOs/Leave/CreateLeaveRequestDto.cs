namespace Application.DTOs.Leave;

public sealed record CreateLeaveRequestDto(
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason);
