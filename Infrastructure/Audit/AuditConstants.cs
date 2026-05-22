namespace Infrastructure.Audit;

public static class AuditEntities
{
    public const string LeaveRequest = "LeaveRequest";
    public const string Authentication = "Authentication";
}

public static class AuditActions
{
    public const string Created = "Created";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Login = "Login";
}
