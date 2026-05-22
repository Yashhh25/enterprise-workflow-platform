namespace Application.DTOs.Auth;

public sealed record AuthResponseDto(
    string Token,
    string Email,
    string Role);
