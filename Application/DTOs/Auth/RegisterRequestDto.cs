namespace Application.DTOs.Auth;

public sealed record RegisterRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Password);
