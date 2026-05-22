namespace Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public Guid TenantId { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
