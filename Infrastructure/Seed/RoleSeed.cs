using Domain.Entities;

namespace Infrastructure.Seed;

public static class RoleSeed
{
    public static readonly Guid EmployeeId = new("a1000001-0001-4001-8001-000000000001");
    public static readonly Guid ManagerId = new("a1000001-0001-4001-8001-000000000002");
    public static readonly Guid HrId = new("a1000001-0001-4001-8001-000000000003");
    public static readonly Guid FinanceId = new("a1000001-0001-4001-8001-000000000004");
    public static readonly Guid AdminId = new("a1000001-0001-4001-8001-000000000005");

    public static readonly Role[] Roles =
    [
        new Role { Id = EmployeeId, Name = "Employee" },
        new Role { Id = ManagerId, Name = "Manager" },
        new Role { Id = HrId, Name = "HR" },
        new Role { Id = FinanceId, Name = "Finance" },
        new Role { Id = AdminId, Name = "Admin" }
    ];
}
