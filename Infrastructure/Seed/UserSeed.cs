using Domain.Entities;

namespace Infrastructure.Seed;

/// <summary>
/// Development-only demo users. Password for all accounts: Test123@
/// </summary>
public static class UserSeed
{
    public const string DemoPassword = "Test123@";

    // Pre-computed BCrypt hash of DemoPassword for migration-safe HasData seeding.
    public const string DemoPasswordHash = "$2a$11$8WY7IQx/VYPXFeXk8VMC0ey5q3J5oAY.1yKet7h9iS3gNRWrv2pDy";

    public static readonly Guid EmployeeUserId = new("c1000001-0001-4001-8001-000000000001");
    public static readonly Guid ManagerUserId = new("c1000001-0001-4001-8001-000000000002");
    public static readonly Guid HrUserId = new("c1000001-0001-4001-8001-000000000003");

    public static readonly User[] Users =
    [
        new User
        {
            Id = EmployeeUserId,
            FirstName = "Demo",
            LastName = "Employee",
            Email = "employee@test.com",
            PasswordHash = DemoPasswordHash,
            RoleId = RoleSeed.EmployeeId,
            TenantId = TenantSeed.DefaultCompanyId,
            CreatedAt = TenantSeed.DefaultCreatedAt
        },
        new User
        {
            Id = ManagerUserId,
            FirstName = "Demo",
            LastName = "Manager",
            Email = "manager@test.com",
            PasswordHash = DemoPasswordHash,
            RoleId = RoleSeed.ManagerId,
            TenantId = TenantSeed.DefaultCompanyId,
            CreatedAt = TenantSeed.DefaultCreatedAt
        },
        new User
        {
            Id = HrUserId,
            FirstName = "Demo",
            LastName = "Hr",
            Email = "hr@test.com",
            PasswordHash = DemoPasswordHash,
            RoleId = RoleSeed.HrId,
            TenantId = TenantSeed.DefaultCompanyId,
            CreatedAt = TenantSeed.DefaultCreatedAt
        }
    ];
}
