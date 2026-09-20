---
name: backend-conventions
description: .NET 8 ASP.NET Core backend code conventions — controller patterns, EF Core, DTOs, service registration, error handling, audit logging. Load when writing or editing any backend code.
---

# Backend Code Conventions

## Project Location
`/home/khaled/app/login-api/`

## Namespace Map

| Layer | Namespace |
|---|---|
| Controllers | `login_api.Controllers` |
| Models/Entities | `login_api.Models` |
| DbContext | `login_api.Data` |
| Services | `login_api.Services` |

## Controller Pattern

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using login_api.Data;
using login_api.Models;
using login_api.Services;

namespace login_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuditService _audit;

    public ItemsController(AppDbContext db, AuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid employeeId)
    {
        var items = await _db.SomeDbSet
            .Where(x => x.EmployeeId == employeeId)
            .Select(x => new ItemDto { ... })
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] Guid employeeId, [FromBody] CreateDto dto)
    {
        // validate
        if (something) return BadRequest(new { message = "Reason" });

        var entity = new Entity { ... };
        _db.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(employeeId, "Create", "Entity", entity.Id, entity.Name, null, "Created");

        return Ok(new { message = "Success", id = entity.Id });
    }

    [HttpPut("{id}/action")]
    public async Task<IActionResult> Action(Guid id, [FromQuery] Guid employeeId)
    {
        var entity = await _db.Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null) return NotFound(new { message = "Not found" });

        // ... update logic ...

        await _db.SaveChangesAsync();
        return Ok(new { message = "Updated." });
    }
}
```

### Key rules
- **File-scoped namespaces** (`namespace x;` — no braces)
- **Route**: `api/[controller]` (lowercase of controller name minus "Controller")
- **DI**: constructor injection, private readonly field `_db` for DbContext
- **Employee lookup**: always `[FromQuery] Guid employeeId`
- **Async**: always `async Task<IActionResult>`
- **Entity IDs**: `Guid` (not int)
- **Empty catch**: allowed for network swallows in frontend only — backend must never use empty catch

## Entity / DTO Conventions

### Entity (maps to DB table)
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace login_api.Models;

[Table("table_name")]
public class EntityName
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("snake_case_column")]
    public string PropertyName { get; set; } = string.Empty;

    [Column("nullable_fk")]
    public Guid? OtherEntityId { get; set; }

    [NotMapped]
    public string Computed => $"{Field1} {Field2}";
}
```

Rules:
- `[Table("snake_case_plural")]` — e.g. `leave_requests`, `attendance_records`
- `[Column("snake_case")]` — every property must map to a DB column
- `string` non-nullable → `= string.Empty`
- `string?` nullable → no default
- `Guid` for PKs and FKs; `Guid?` for optional FKs
- `DateOnly` for dates, `TimeOnly?` for nullable times, `DateTime` for timestamps

### DTO (API response/request)
```csharp
namespace login_api.Models;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? OptionalField { get; set; }
}
```

Rules:
- No `[Column]` attributes (DTOs aren't DB-mapped)
- PascalCase properties — ASP.NET serializes to camelCase JSON automatically
- Use `string?` for optional fields, `string` with `= string.Empty` for required

## Service Registration (Program.cs)

```csharp
builder.Services.AddScoped<AuditService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

- Custom services: `AddScoped`
- DbContext: `AddDbContext` with `UseNpgsql`
- CORS policy `"AllowFrontend"` pinned to `http://localhost:5173`

## DbContext (Data/AppDbContext.cs)

```csharp
public DbSet<EntityName> EntityNames { get; set; }  // plural of entity
```

- DbSet name = plural of entity class name
- No `OnModelCreating` — all mappings via `[Table]`/`[Column]` attributes

## Audit Logging

Every create/update/delete operation that affects employee data should log:

```csharp
await _audit.LogAsync(employeeId, "ActionName", "EntityType", entityId, entityName, oldValue, "Description");
```

Parameters: `(actionBy, action, entityType, entityId, entityName, oldValue, details)`

## Error Handling

- Validation errors: `return BadRequest(new { message = "Clear reason" })`
- Not found: `return NotFound(new { message = "Not found" })`
- Access denied: `return StatusCode(403, new { message = "Access denied. HR role required." })`
- Success: `return Ok(new { message = "Done." })` or `return Ok(dto)`
- Never expose raw exceptions to the client

## Approval Chain Pattern

- **Leave**: Supervisor (step 1) → HR (step 2) — tracks via `current_approver_role_level` (1=supervisor, 2=HR)
- **Mission**: Single-step (either supervisor or HR)
- On approve at final step: call `_audit.LogAsync(...)` and create `Notification` for the employee
- On approve at intermediate step: reassign `CurrentApproverId` to next approver
- On reject: set status to "Rejected", notify employee
