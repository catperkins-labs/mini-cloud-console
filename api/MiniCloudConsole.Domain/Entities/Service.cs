using MiniCloudConsole.Domain.Enums;

namespace MiniCloudConsole.Domain.Entities;

public class Service
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Project Project { get; set; } = null!;
}
