# Pesyong Catering - Solution README

Full-stack catering management solution. This repository contains backend APIs, domain model, infrastructure, contracts, an admin web presentation, and MAUI client projects.

## Solution overview
Purpose: Provide APIs and client apps to manage catering packages, sizes, selection rules, addons, orders, and dashboards for admins and customers.

High-level architecture:
- CMS.Domain: core domain models and domain logic.
- CMS.Contracts: DTOs and request/response contracts used across layers.
- CMS.Application: application services, business use-cases, and orchestrations.
- CMS.Infrastructure: EF Core DbContext, repository implementations, data access, migrations.
- CMS.Server: ASP.NET Core Web API exposing REST endpoints (admin + public).
- PESYONG.Presentation.Admin: Admin web UI (Blazor/React/ASP.NET Razor — adjust if different).
- MauiApp-Pesyong.Shared: Shared code for MAUI apps (models, helpers).
- MauiApp-Pesyong: Mobile client (MAUI) for customers.
- MauiApp-Pesyong.Web: Web client/host for MAUI or PWA.

Projects
- CMS.Domain (CMS.Domain.csproj)
  - Entities: Package, PackageSize, PackageSelectionRule, PackageSelectionOption, PackageAddon, Order, OrderItem, etc.
  - Domain validations and value objects.
- CMS.Contracts (CMS.Contracts.csproj)
  - DTOs used by controllers and clients, request/response shapes.
- CMS.Application (CMS.Application.csproj)
  - Application-level services, validators, mappers, and business workflows.
- CMS.Infrastructure (CMS.Infrastructure.csproj)
  - EF Core DbContext, migrations, repository implementations, seed data.
- CMS.Server (CMS.Server.csproj)
  - ASP.NET Core Web API controllers, middleware, authentication, SignalR or stat broadcaster.
- PESYONG.Presentation.Admin (PESYONG.Presentation.Admin.csproj)
  - Admin UI project to manage packages, orders, and dashboard.
- MauiApp-Pesyong.Shared (MauiApp-Pesyong.Shared.csproj)
  - Shared models and helpers used by mobile & web clients.
- MauiApp-Pesyong (MauiApp-Pesyong.csproj)
  - MAUI mobile application (Android/iOS) for customers.
- MauiApp-Pesyong.Web (MauiApp-Pesyong.Web.csproj)
  - Web-host for MAUI app or PWA.

Prerequisites
- .NET SDK (version used by the solution, e.g., .NET 7)
- SQL Server (local or container)
- dotnet-ef (for migrations) if you will run migrations locally

Setup and local run

1. Clone
   git clone <repo-url>
   cd <repo-root>

2. Configure connection string
   Edit CMS.Server/appsettings.Development.json or use environment variable:
   - ConnectionStrings:DefaultConnection = "Server=.;Database=PesyongCmsDb;Trusted_Connection=True;"

3. Apply database migrations (from solution root)
   `dotnet tool restore dotnet ef database update --project CMS.Infrastructure --startup-project CMS.Server

4. Run the API
   `dotnet run --project CMS.Server

5. Run the admin UI
- If it's a Blazor/ASP.NET app:
  ` dotnet run --project PESYONG.Presentation.Admin
  - If it's an SPA (npm based) follow the README inside the presentation project.

6. Run MAUI apps
- Open MauiApp-Pesyong.sln in Visual Studio 2022/2023 and run for desired target (Android/iOS/Windows/Web).

Build
` dotnet build


Testing
- If unit/integration tests exist, run:
  ` dotnet test


Key endpoints (examples)
- PUT /api/admin/packages/{id} - update package with sizes, rules, options, addons
- POST /api/orders - create order
- GET /api/packages - list packages

Known issues & recommendations
- Deleting nested PackageSizes that are referenced by OrderItems can trigger FK conflicts. Consider:
  - Prevent removal when referenced (409 response),
  - Use soft-delete for historical consistency,
  - Migrate FK delete behavior carefully if changing to cascade/set null.
- Validate large object graphs with care to avoid performance issues (use paging, projection).

Development tips
- Use eager loading (Include/ThenInclude) only where needed to avoid N+1.
- Keep DTOs in CMS.Contracts to decouple clients from EF entities.
- Apply database migrations from CMS.Infrastructure; use CMS.Server as startup project.

Contribution
- Fork → branch → commit → PR
- Add tests for new behavior and update migrations when schema changes.

License
- Add your chosen license file (e.g., MIT) at repository root.

Contact / Maintainers
- Add maintainers/team contact info here.

