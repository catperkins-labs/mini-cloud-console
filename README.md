# mini-cloud-console

A lightweight cloud management console scaffold built with:

- **Backend**: .NET 8 Minimal API + EF Core 8 (Npgsql) + PostgreSQL
- **Frontend**: React + TypeScript (Vite)
- **Local dev**: Docker Compose runs Postgres + API; frontend runs via npm on the host

---

## Project structure

```
mini-cloud-console/
├── docker-compose.yml
├── .env.example
├── api/
│   ├── MiniCloudConsole.sln
│   ├── MiniCloudConsole.Api/          # Minimal API, JWT auth, DI wiring
│   ├── MiniCloudConsole.Data/         # AppDbContext, EF migrations, seed helper
│   └── MiniCloudConsole.Domain/       # Entities & enums
└── web/                               # Vite React TS app
    └── src/
        ├── api/client.ts              # Typed fetch wrapper + API base URL config
        └── pages/                     # LoginPage, OrgsPage, OrgPage, ProjectPage
```

---

## Getting started

### 1. Configure environment variables

```bash
cp .env.example .env
# Edit .env – set POSTGRES_PASSWORD and JWT_SIGNING_KEY
```

### 2. Start the API and database

```bash
docker compose up --build
```

The API will be available at <http://localhost:8080>.  
Swagger UI (dev mode): <http://localhost:8080/swagger>.

> **Migrations & seed data** are applied automatically on first startup when
> `ASPNETCORE_ENVIRONMENT=Development`.

### 3. Start the frontend

```bash
cd web
cp .env.example .env.local    # optional – defaults to http://localhost:8080
npm install
npm run dev
```

The web app will be available at <http://localhost:5173>.

---

## Planned API endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `GET`  | `/health` | No | Health check |
| `POST` | `/auth/dev-login` | No | Dev-only: issue JWT for an email (no password) |
| `GET`  | `/orgs` | Yes | List organizations |
| `GET`  | `/orgs/{orgId}` | Yes | Get an organization |
| `GET`  | `/orgs/{orgId}/members` | Yes | List org members |
| `POST` | `/orgs` | Yes | Create an organization *(TODO)* |
| `GET`  | `/orgs/{orgId}/projects` | Yes | List projects in an org |
| `GET`  | `/orgs/{orgId}/projects/{projectId}` | Yes | Get a project |
| `POST` | `/orgs/{orgId}/projects` | Yes | Create a project *(TODO)* |
| `GET`  | `/projects/{projectId}/services` | Yes | List services in a project |
| `POST` | `/projects/{projectId}/services` | Yes | Create a service *(TODO)* |
| `PATCH`| `/projects/{projectId}/services/{serviceId}` | Yes | Update service status *(TODO)* |

---

## Domain model

| Entity | Key fields |
|--------|-----------|
| `Organization` | Id, Name, Slug, CreatedAt |
| `User` | Id, Email, DisplayName, CreatedAt |
| `OrgMembership` | Id, OrganizationId, UserId, Role (`Member`/`Admin`/`Owner`) |
| `Project` | Id, OrganizationId, Name, Slug, CreatedAt |
| `Service` | Id, ProjectId, Name, Status (`Pending`/`Running`/`Stopped`/`Failed`) |
| `AuditEvent` | Id, UserId?, Action, ResourceType, ResourceId?, Metadata?, OccurredAt |

---

## Running the backend standalone (without Docker)

```bash
cd api
# Ensure a local Postgres instance is running and update appsettings.Development.json
dotnet run --project MiniCloudConsole.Api
```
