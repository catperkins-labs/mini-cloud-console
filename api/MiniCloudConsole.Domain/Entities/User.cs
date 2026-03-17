namespace MiniCloudConsole.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<OrgMembership> Memberships { get; set; } = new List<OrgMembership>();
    public ICollection<AuditEvent> AuditEvents { get; set; } = new List<AuditEvent>();
}
