---
name: db-conventions
description: PostgreSQL code conventions — table/column naming, data types, FK patterns, migration scripts, and common queries. Load when writing SQL, designing schemas, or debugging DB issues.
---

# Database Conventions

## Connection
- **Host**: localhost (Docker container `login-pg`)
- **Database**: `logindb`
- **User**: `loginuser`
- **Port**: 5432

## Quick Access
```bash
docker exec login-pg psql -U loginuser -d logindb -c "SQL STATEMENT"
docker exec -it login-pg psql -U loginuser -d logindb   # interactive
```

## Naming Conventions

| Item | Convention | Example |
|---|---|---|
| **Table names** | snake_case, plural | `employees`, `leave_requests`, `attendance_records` |
| **Column names** | snake_case | `employee_id`, `first_name`, `created_at` |
| **Primary key** | always `id` (uuid) | `id UUID DEFAULT uuid_generate_v4()` |
| **Foreign key** | `{referenced_table_singular}_id` | `employee_id`, `supervisor_id`, `approved_by` |
| **Timestamps** | `created_at` (TIMESTAMPTZ) | `created_at TIMESTAMPTZ DEFAULT now()` |
| **Boolean** | `is_` prefix | `is_active`, `is_read` |
| **Status** | `status VARCHAR(20)` | 'Pending', 'Approved', 'Rejected', 'Cancelled' |

## Data Types

| C# Type | PostgreSQL Type | Notes |
|---|---|---|
| `Guid` | `uuid` | Default with `uuid_generate_v4()` |
| `string` | `VARCHAR(n)` or `TEXT` | `VARCHAR(20)` for codes/status, `TEXT` for long text |
| `int` | `integer` | |
| `decimal` | `NUMERIC(p,s)` | `NUMERIC(5,1)` for leave balances |
| `DateOnly` | `date` | |
| `TimeOnly` | `time without time zone` | |
| `DateTime` | `timestamp with time zone` | Always use timestamptz |
| `bool` | `boolean` | |

## Foreign Key Pattern

```sql
CREATE TABLE child_table (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    employee_id UUID NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    -- For nullable references (approvers, reviewers):
    approved_by UUID REFERENCES employees(id),
    created_at TIMESTAMPTZ DEFAULT now()
);
```

Rule: FKs that represent **ownership** (`employee_id`) use `ON DELETE CASCADE`. FKs that represent **references** (`approved_by`, `reviewed_by`) have no cascade — the backend code sets them to `NULL` before deleting.

## Migration Scripts

Migration files live in `login-api/database/` as numbered SQL files:

```
database/
├── 001_initial.sql
├── 002_add_leave_balances.sql
└── ...
```

Run a migration:
```bash
docker exec -i login-pg psql -U loginuser -d logindb < login-api/database/NNN_name.sql
```

## Common Queries

### Check table schema
```bash
docker exec login-pg psql -U loginuser -d logindb -c "\d table_name"
```

### List all tables
```bash
docker exec login-pg psql -U loginuser -d logindb -c "\dt"
```

### Count FK references to an employee
```sql
SELECT 'leave_requests' as tbl, COUNT(*) FROM leave_requests WHERE employee_id = '<uuid>'
UNION ALL
SELECT 'missions', COUNT(*) FROM mission_requests WHERE employee_id = '<uuid>';
```

### Find subordinates of an employee
```sql
SELECT id, first_name, last_name FROM employees WHERE supervisor_id = '<uuid>';
```

### Seed data pattern (used for holidays)
```sql
INSERT INTO holidays (date, name) VALUES
('2026-01-01', 'New Year'),
('2026-04-25', 'Sinai Liberation Day')
ON CONFLICT (date) DO NOTHING;
```

## EF Core Mapping (C# side)

- `[Table("snake_case_plural")]` on entity class
- `[Column("snake_case")]` on every property
- `[Key]` on `Id` property
- `[NotMapped]` on computed properties (`FullName => $"{FirstName} {LastName}"`)
- `DatabaseGenerated(DatabaseGeneratedOption.Identity)` for auto-generated UUIDs

See `backend-conventions` skill for full entity examples.

## Indexes

Add indexes for frequently queried columns:
```sql
CREATE INDEX idx_leave_requests_employee_id ON leave_requests(employee_id);
CREATE INDEX idx_attendance_records_employee_date ON attendance_records(employee_id, date);
```

The EF Core `[Index]` attribute can also be used on entity classes for index definitions.
