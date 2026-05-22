namespace Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string entityName,
        string action,
        Guid userId,
        Guid tenantId,
        string description,
        CancellationToken cancellationToken = default);
}
