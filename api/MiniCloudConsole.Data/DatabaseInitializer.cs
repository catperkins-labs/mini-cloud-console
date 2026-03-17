using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniCloudConsole.Domain.Entities;

namespace MiniCloudConsole.Data;

public static class DatabaseInitializer
{
    public static async Task MigrateAndSeedAsync(AppDbContext db, ILogger logger)
    {
        logger.LogInformation("Applying pending migrations...");
        await db.Database.MigrateAsync();

        if (!await db.Organizations.AnyAsync())
        {
            logger.LogInformation("Seeding default data...");

            var org = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Default Org",
                Slug = "default",
                CreatedAt = DateTimeOffset.UtcNow,
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                DisplayName = "Admin",
                CreatedAt = DateTimeOffset.UtcNow,
            };

            var membership = new OrgMembership
            {
                Id = Guid.NewGuid(),
                Organization = org,
                User = user,
                Role = Domain.Enums.OrgRole.Owner,
                JoinedAt = DateTimeOffset.UtcNow,
            };

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Organization = org,
                Name = "Sample Project",
                Slug = "sample",
                CreatedAt = DateTimeOffset.UtcNow,
            };

            db.Organizations.Add(org);
            db.Users.Add(user);
            db.OrgMemberships.Add(membership);
            db.Projects.Add(project);

            await db.SaveChangesAsync();
            logger.LogInformation("Seed complete.");
        }
    }
}
