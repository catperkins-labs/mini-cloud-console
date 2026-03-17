using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniCloudConsole.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? throw new InvalidOperationException("Database connection string is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:SigningKey"]
    ?? Environment.GetEnvironmentVariable("JWT_SIGNING_KEY")
    ?? throw new InvalidOperationException("JWT signing key is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "mini-cloud-console",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "mini-cloud-console",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services.AddAuthorization();

// ── OpenAPI / Swagger ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── CORS (dev permissive) ─────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// ── Dev: apply migrations + seed ──────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
    await DatabaseInitializer.MigrateAndSeedAsync(db, logger);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ─────────────────────────────────────────────────────────────────

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("HealthCheck")
    .WithOpenApi();

// Dev-login stub – issues a token for a given email (DEV ONLY, no password check)
app.MapPost("/auth/dev-login", (DevLoginRequest req, IConfiguration config) =>
{
    // TODO: replace with real authentication
    var key = config["Jwt:SigningKey"] ?? Environment.GetEnvironmentVariable("JWT_SIGNING_KEY")!;
    var issuer = config["Jwt:Issuer"] ?? "mini-cloud-console";
    var audience = config["Jwt:Audience"] ?? "mini-cloud-console";

    var claims = new[]
    {
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, req.Email),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, req.Email),
    };

    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256)
    );

    var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
})
.WithName("DevLogin")
.WithOpenApi()
.AllowAnonymous();

// Organizations
app.MapGet("/orgs", (AppDbContext db) =>
    db.Organizations.Select(o => new { o.Id, o.Name, o.Slug, o.CreatedAt }).ToListAsync())
    .RequireAuthorization()
    .WithName("ListOrgs")
    .WithOpenApi();

app.MapGet("/orgs/{orgId:guid}", async (Guid orgId, AppDbContext db) =>
    await db.Organizations.FindAsync(orgId) is { } org
        ? Results.Ok(org)
        : Results.NotFound())
    .RequireAuthorization()
    .WithName("GetOrg")
    .WithOpenApi();

// Projects
app.MapGet("/orgs/{orgId:guid}/projects", (Guid orgId, AppDbContext db) =>
    db.Projects.Where(p => p.OrganizationId == orgId)
        .Select(p => new { p.Id, p.Name, p.Slug, p.CreatedAt }).ToListAsync())
    .RequireAuthorization()
    .WithName("ListProjects")
    .WithOpenApi();

app.MapGet("/orgs/{orgId:guid}/projects/{projectId:guid}", async (Guid orgId, Guid projectId, AppDbContext db) =>
    await db.Projects.FirstOrDefaultAsync(p => p.OrganizationId == orgId && p.Id == projectId) is { } project
        ? Results.Ok(project)
        : Results.NotFound())
    .RequireAuthorization()
    .WithName("GetProject")
    .WithOpenApi();

// Services
app.MapGet("/projects/{projectId:guid}/services", (Guid projectId, AppDbContext db) =>
    db.Services.Where(s => s.ProjectId == projectId)
        .Select(s => new { s.Id, s.Name, s.Status, s.CreatedAt }).ToListAsync())
    .RequireAuthorization()
    .WithName("ListServices")
    .WithOpenApi();

// Users / Members
app.MapGet("/orgs/{orgId:guid}/members", (Guid orgId, AppDbContext db) =>
    db.OrgMemberships.Where(m => m.OrganizationId == orgId)
        .Select(m => new { m.Id, m.UserId, m.User.Email, m.User.DisplayName, m.Role, m.JoinedAt })
        .ToListAsync())
    .RequireAuthorization()
    .WithName("ListOrgMembers")
    .WithOpenApi();

app.Run();

record DevLoginRequest(string Email);
