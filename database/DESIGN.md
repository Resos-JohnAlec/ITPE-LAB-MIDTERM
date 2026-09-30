# 1. Design Assumptions

SQL Server LocalDB is the selected DBMS. Three entities are enough. Students are identified by normalized, case-insensitively unique email; no user account or password is added. The allowed application domain is `dlsud.edu.ph`. Event administration is out of scope, and sample events are seeded. DATETIMEOFFSET retains time-zone information; display is Philippine time. Users are identified by self-supplied data in this local prototype.

# 2. 3NF Table Design

| Table | Column | SQL type | Null? | Rule |
|---|---|---|---|---|
| Users | UserId | INT IDENTITY | No | Clustered PK |
| Users | FullName | NVARCHAR(100) | No | Trimmed length 2–100 |
| Users | Email | NVARCHAR(254) | No | Case-insensitive unique key, lowercase, basic shape check |
| Events | EventId | INT IDENTITY | No | Clustered PK |
| Events | Title | NVARCHAR(150) | No | Nonblank |
| Events | Description | NVARCHAR(1000) | No | Nonblank |
| Events | Location | NVARCHAR(150) | No | Nonblank |
| Events | StartsAt | DATETIMEOFFSET(0) | No | Stored instant and offset |
| Events | Capacity | INT | No | 1–10,000 |
| Registrations | RegistrationId | INT IDENTITY | No | Clustered PK |
| Registrations | UserId | INT | No | FK to Users; part of unique pair |
| Registrations | EventId | INT | No | FK to Events; part of unique pair |
| Registrations | RegisteredAt | DATETIMEOFFSET(0) | No | Defaults to server timestamp |

Every column is atomic (1NF). Non-key attributes depend on the whole candidate key (2NF), and there is no non-key attribute determining another non-key attribute within a table (3NF). A registration stores references, not duplicate user names, emails, event titles, or capacities. Available seats are calculated from Capacity minus COUNT(Registrations), not stored. Description is descriptive event text, not a separate related entity for this scope.

# 3. Mermaid ERD

The canonical source is `erd.mmd`; the same diagram is embedded in root `SUBMISSION.md`. Users and Events each have a one-to-many relationship with Registrations; each registration must reference exactly one of each.

# 4. SQL DDL Script

`schema.sql` is executable SQL Server DDL. The backend `--init-db` command creates a dedicated configured database, executes this exact script, then runs `seed.sql`. Alternatively create a database in SQL Server Management Studio, select it, and execute the script followed by seed.sql. No `GO` client directive is needed. Re-running initialization preserves existing records; this is initialization, not a versioned migration engine.

# 5. Constraint and Index Summary

| Constraint/index | Purpose |
|---|---|
| PK_Users / PK_Events / PK_Registrations | Stable identities |
| UQ_Users_Email (non-clustered) | Unique normalized student identity; lookup by email |
| UQ_Registrations_User_Event (non-clustered) | No repeated student/event pair; leading UserId indexes that FK |
| IX_Registrations_EventId (non-clustered, includes UserId and RegisteredAt) | Capacity counts, attendee queries, and EventId FK indexing |
| IX_Events_StartsAt (non-clustered) | Upcoming catalog selection/order |
| FK_Registrations_Users / FK_Registrations_Events | Both ON DELETE NO ACTION / ON UPDATE NO ACTION; retain history and stable IDs |
| CHECK constraints | Names/text must be nonblank, email has basic normalized shape, capacity is positive and bounded |

No redundant separate UserId index is necessary: that FK is already the first key column of a unique non-clustered index. A CHECK cannot enforce a count across related rows in SQL Server; the application locks the event and performs capacity checking and insertion within a transaction. Start-time and exact-domain rules are also applied by the service; future events cannot be a permanent CHECK condition because time advances.

# 6. Short Verification Checklist

Verify the script executes in LocalDB, three tables exist, duplicate pairs and invalid FKs fail, capacity zero fails, parent deletion with attendance fails, and both FKs lead a non-clustered index. Run the integration project against a dedicated verification database to exercise those checks and eight concurrent requests for one seat. Confirm ERD names, keys, and cardinalities against this table design. Human review and actual execution evidence are recorded separately in `SUBMISSION.md`.
