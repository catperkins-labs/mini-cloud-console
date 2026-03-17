namespace MiniCloudConsole.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<OrgMembership> Memberships { get; set; } = new List<OrgMembership>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
