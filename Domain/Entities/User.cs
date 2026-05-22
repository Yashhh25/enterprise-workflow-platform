namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public Guid TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}