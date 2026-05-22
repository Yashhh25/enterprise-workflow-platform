using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
namespace Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private readonly ApplicationDbContext _dbContext;

    public AuditService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(
        string entityName,
        string action,
        Guid userId,
        Guid tenantId,
        string description,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityName = entityName.Trim(),
            Action = action.Trim(),
            UserId = userId,
            TenantId = tenantId,
            Description = description.Trim(),
            Timestamp = DateTime.UtcNow
        };

        _dbContext.AuditLogs.Add(auditLog);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
