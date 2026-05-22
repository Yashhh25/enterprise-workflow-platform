using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Audit;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    public const string UserIdClaimType = "userId";

    public const string TenantIdClaimType = "tenantId";

    private const string DefaultRoleName = "Employee";

    private readonly ApplicationDbContext _dbContext;
    private readonly JwtSettings _jwtSettings;
    private readonly IAuditService _auditService;

    public AuthService(
        ApplicationDbContext dbContext,
        IOptions<JwtSettings> jwtSettings,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _jwtSettings = jwtSettings.Value;
        _auditService = auditService;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);

        if (await _dbContext.Users.AnyAsync(
                u => u.TenantId == TenantSeed.DefaultCompanyId && u.Email == email,
                cancellationToken))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var employeeRole = await _dbContext.Roles
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Name == DefaultRoleName, cancellationToken)
            ?? throw new InvalidOperationException($"Default role '{DefaultRoleName}' was not found.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = employeeRole.Id,
            TenantId = TenantSeed.DefaultCompanyId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            GenerateJwtToken(user, DefaultRoleName),
            user.Email,
            DefaultRoleName);
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);

        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        await _auditService.LogAsync(
            AuditEntities.Authentication,
            AuditActions.Login,
            user.Id,
            user.TenantId,
            $"User {user.Email} logged in successfully.",
            cancellationToken);

        return new AuthResponseDto(
            GenerateJwtToken(user, user.Role.Name),
            user.Email,
            user.Role.Name);
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private string GenerateJwtToken(User user, string roleName)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(UserIdClaimType, user.Id.ToString()),
            new Claim(TenantIdClaimType, user.TenantId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roleName)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
