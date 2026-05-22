using Domain.Entities;

namespace Infrastructure.Seed;

public static class TenantSeed
{
    public static readonly Guid DefaultCompanyId = new("b1000001-0001-4001-8001-000000000001");

    public static readonly DateTime DefaultCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public const string DefaultCompanyName = "Default Company";

    public const string DefaultCompanySlug = "default-company";

    public static readonly Tenant[] Tenants =
    [
        new Tenant
        {
            Id = DefaultCompanyId,
            Name = DefaultCompanyName,
            Slug = DefaultCompanySlug,
            CreatedAt = DefaultCreatedAt
        }
    ];
}
