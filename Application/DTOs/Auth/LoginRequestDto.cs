namespace Application.DTOs.Auth;

public sealed record LoginRequestDto(
    string Email,
    string Password);
