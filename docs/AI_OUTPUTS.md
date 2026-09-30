# AI output evidence

This is a verbatim snapshot of the final AI-authored deliverable files, assembled mechanically for documentation. It is not an invented chat transcript or a claim that each adopted prompt was submitted separately. Task 1 preserves the original architecture output; other files include corrections made during implementation and verification. Use the source files as the executable artifacts and the raw reports in docs/evidence as execution evidence. Human changes made later must be recorded separately.

## docs/task-1-ai-output.md

`````markdown
# Overall System Design

## 1. Requirements Summary

Build a local, working Online Campus Event Management System that lets students view upcoming events and register, and lets an administrator view each event's attendees. Keep the prototype small enough for a three-person team to understand and demonstrate within three hours.

## 2. Functional Requirements

| ID | Requirement | Implementation boundary |
|---|---|---|
| FR1 | List upcoming events with title, description, location, date/time, and available seats | Event catalog and GET /api/events |
| FR2 | Register a named student with a valid university email | Registration form and POST /api/registrations |
| FR3 | Display registered attendees for a selected event | Administrator page and protected GET /api/admin/events/{id}/attendees |
| FR4 | Reject duplicate registrations, invalid inputs, started events, and full events | Server validation, database uniqueness, and transaction |

## 3. Non-functional Requirements

Use semantic HTML, labeled controls, visible keyboard focus, readable contrast, and textual validation feedback. Persist records in a relational database. Parameterize database input and dispose connections and commands. Keep error responses understandable without exposing database details. Separate the UI, service, SQL, tests, and documentation so team members can work independently.

## 4. Technology Recommendations

| Layer | Choice | Reason |
|---|---|---|
| Frontend | Plain HTML5, CSS, JavaScript modules | No bundler or UI framework setup; sufficient for two small pages |
| Backend | .NET 10 ASP.NET Core Minimal API | Matches the examination's C# deliverable and provides a small HTTP boundary |
| Database | SQL Server LocalDB with explicit SQL | Matches SqlConnection and explicit non-clustered indexes in the prompt pack |
| Unit tests | xUnit and Moq | Familiar Arrange-Act-Assert tests with a mocked domain-policy dependency |
| Integration checks | PowerShell/HTTP and browser automation | Exercise the actual database, API, and student/admin flow separately from unit tests |

Serve the frontend from the backend's origin to avoid a second server and CORS configuration. Use Microsoft.Data.SqlClient directly; an ORM is unnecessary for three tables.

## 5. Architecture and Data Flow

```text
Student / administrator browser
            |
    HTML + CSS + JavaScript
            | same-origin JSON requests
    ASP.NET Core Minimal API
            |
    Validation + RegistrationService
            | parameterized commands / registration transaction
    SQL Server LocalDB
       Users -- Registrations -- Events
```

The catalog queries future events and calculates availability from capacity minus registration count. A registration request validates the student's name and exact university email domain, locks the event within a transaction, checks capacity and duplicates, finds or creates the user, and inserts the registration. Only after commit does the UI report success. An administrator supplies a locally configured demo access key before attendee data is returned.

## 6. Components

The event catalog renders cards and handles loading, empty, and retry states. The registration form displays the selected event, validates input, submits to the API, and refreshes seats. The administrator page chooses an event and renders a labeled attendee table. EmailValidator depends on an IUniversityDomainPolicy so unit tests can replace configuration with a mock. RegistrationService owns the SQL queries and transaction. DatabaseBootstrap applies schema and optional demo seed data to a dedicated database.

## 7. Entities and Relationships

Users stores UserId, FullName, and a unique normalized Email. Events stores EventId, Title, Description, Location, StartsAt, and Capacity. Registrations stores RegistrationId, UserId, EventId, and RegisteredAt. Each user and event can have many registrations; a unique UserId/EventId pair prevents duplicates. Do not repeat an email in Registrations or persist derived available-seat counts. This produces three tables in Third Normal Form.

## 8. API Boundaries

| Method | Path | Result |
|---|---|---|
| GET | /api/config | Public university email-domain configuration |
| GET | /api/events | Upcoming event catalog and derived available seats |
| POST | /api/registrations | 201 confirmation; 400 invalid input; 404 missing event; 409 duplicate/full/started |
| GET | /api/admin/events | Event choices, including past events; requires admin key |
| GET | /api/admin/events/{id}/attendees | Attendee list; requires admin key |

Use clear JSON error messages and explicit status codes. The admin key is a small local-demo access guard, not a full account or login system.

## 9. Prioritized Implementation Plan

First agree on schema, DTO field names, university domain, and endpoint paths. Then create SQL and the C# service, followed by the catalog/form and administrator view. Add isolated validation tests and integration checks for duplicates and capacity. Finally execute the schema, build and test, review accessibility, and consolidate evidence and prompts in SUBMISSION.md.

## 10. Assumptions, Risks, and Time-saving Decisions

Assumptions: this is a local classroom prototype; SQL Server LocalDB is available on Windows; registration accepts the exact dlsud.edu.ph domain supplied by the user; three team members share QA; events are seeded sample activities rather than official university announcements. Store event timestamps as datetimeoffset and display them in Asia/Manila. Dates in seed data are relative to initialization so the first demonstration has upcoming events.

Email-domain validation does not prove identity or email ownership. The administrator key must stay out of source control and browser persistence. This prototype is not ready for public deployment without actual authentication and authorization. Concurrency is a real correctness risk: capacity checks and insertion must occur in one transaction with an event lock. Do not silently rename an existing user when the same email is submitted again. Full event administration, payments, notifications, analytics, SSO, and distributed infrastructure are deferred.

## 11. Repository Structure

```text
frontend/          Semantic catalog, registration form, admin page, CSS and JS
backend/           API, validation, RegistrationService.cs, database bootstrap
database/          schema.sql, seed.sql, erd.mmd, design notes
tests/             xUnit mock tests and integration/browser checks
scripts/           Local setup, run, verification, evidence assembly
docs/              Exact adopted prompts, AI outputs, security review, evidence
SUBMISSION.md      Consolidated examination report
README.md          Quick start and deliverable map
```

## 3-Hour Prototype Feasibility

Implement the schema, catalog, transaction-safe registration, and attendee view first because they demonstrate all three required capabilities. Share fixed API contracts between frontend and backend work, and reserve the final portion of the time for integration and documentation. Defer every feature outside those three capabilities; complete mocked tests, the security refactor, and honest verification evidence before submission.
`````

## docs/frontend-notes.md

`````markdown
# Task 2 — Interface and accessibility

## Page structure

`frontend/index.html` contains the student catalog and a native registration dialog. Header/nav, main, section, article (created by JavaScript), and footer describe the document. Cards show a title, event time, location, description, derived remaining seats, and registration action. Search narrows the loaded catalog. A labeled form validates full name and university email before submitting to the actual API. The success view displays the server-issued registration ID. `admin.html` provides the required attendee view with a locally configured organizer key.

The complete implementation is in `index.html`, `styles.css`, `api.js`, `app.js`, `admin.html`, and `admin.js`. No frontend build or runtime library is required. The ASP.NET Core host serves these files on the API origin. The event data is fictional seed content read from a real SQL Server database; the browser does not simulate successful persistence.

## Accessibility and POUR mapping

| Principle | Implementation | Verification boundary |
|---|---|---|
| Perceivable | High-contrast semantic palette; visible form labels; event text also contains all decorative-art information; textual errors | Automated axe scans and screenshot inspection; human visual/assistive-technology review remains |
| Operable | Native buttons, links, dialog, input, select; skip link; focus rings; keyboard opening and Escape; disabled submission while pending | Browser keyboard/focus checks, responsive viewport checks |
| Understandable | Specific inline errors, exact university-domain hint, labeled required inputs, explicit loading/empty/error/success messages | Wrong-domain, empty input, duplicate, and success flow automation |
| Robust | Semantic landmarks/headings; label associations; aria-invalid, aria-describedby, role=status/alert; native dialog accessible title | Automated WCAG A/AA scans for catalog, form, success, and both administrator states |

All graphics are decorative CSS compositions or text inside aria-hidden regions; there are no informative raster images needing alt text. Body copy and controls use native system fonts to avoid external font dependencies. Reduced-motion preferences are honored. The search field has an explicit accessible name rather than relying only on placeholder text.

## Design decisions

The UI/UX skill informed the focus, label, contrast, loading, responsive-layout, and motion checks. The final visual direction uses a deep green/cream palette, an editorial hero, and restrained event cards suited to the DLSU-D campus context. It adapts the skill's suggestions to plain HTML/CSS, without adding animation libraries, a framework, unofficial university marks, or unnecessary features.

## Assumptions

The university domain is `dlsud.edu.ph`, supplied by the user. Events are fictional samples, displayed in Asia/Manila. Email ownership is not verified. This is a local classroom prototype. An organizer key provides a minimal attendee-data guard rather than a full login system. Dates and counts are supplied by the database, not hardcoded into the UI.
`````

## database/DESIGN.md

`````markdown
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
`````

## database/erd.mmd

`````mermaid
erDiagram
    Users ||--o{ Registrations : makes
    Events ||--o{ Registrations : receives
    Users {
        int UserId PK
        nvarchar100 FullName
        nvarchar254 Email UK
    }
    Events {
        int EventId PK
        nvarchar150 Title
        nvarchar1000 Description
        nvarchar150 Location
        datetimeoffset StartsAt
        int Capacity
    }
    Registrations {
        int RegistrationId PK
        int UserId FK "Composite unique with EventId"
        int EventId FK "Composite unique with UserId"
        datetimeoffset RegisteredAt
    }
`````

## database/schema.sql

`````sql
-- SQL Server / LocalDB. Run within a dedicated database. Non-destructive on repeat runs.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY CLUSTERED,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(254) COLLATE Latin1_General_100_CI_AS NOT NULL,
        CONSTRAINT UQ_Users_Email UNIQUE NONCLUSTERED (Email),
        CONSTRAINT CK_Users_FullName CHECK (LEN(LTRIM(RTRIM(FullName))) BETWEEN 2 AND 100),
        -- SQL checks basic shape; the application enforces the configured exact domain.
        CONSTRAINT CK_Users_Email CHECK (Email LIKE N'%_@_%._%' AND Email NOT LIKE N'% %'
            AND Email = LOWER(Email) COLLATE Latin1_General_100_BIN2)
    );
END;

IF OBJECT_ID(N'dbo.Events', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Events (
        EventId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Events PRIMARY KEY CLUSTERED,
        Title NVARCHAR(150) NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        Location NVARCHAR(150) NOT NULL,
        StartsAt DATETIMEOFFSET(0) NOT NULL,
        Capacity INT NOT NULL,
        CONSTRAINT CK_Events_Title CHECK (LEN(LTRIM(RTRIM(Title))) > 0),
        CONSTRAINT CK_Events_Description CHECK (LEN(LTRIM(RTRIM(Description))) > 0),
        CONSTRAINT CK_Events_Location CHECK (LEN(LTRIM(RTRIM(Location))) > 0),
        CONSTRAINT CK_Events_Capacity CHECK (Capacity BETWEEN 1 AND 10000)
    );
    CREATE NONCLUSTERED INDEX IX_Events_StartsAt ON dbo.Events (StartsAt);
END;

IF OBJECT_ID(N'dbo.Registrations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Registrations (
        RegistrationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Registrations PRIMARY KEY CLUSTERED,
        UserId INT NOT NULL,
        EventId INT NOT NULL,
        RegisteredAt DATETIMEOFFSET(0) NOT NULL
            CONSTRAINT DF_Registrations_RegisteredAt DEFAULT SYSDATETIMEOFFSET(),
        CONSTRAINT FK_Registrations_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
            ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT FK_Registrations_Events FOREIGN KEY (EventId) REFERENCES dbo.Events (EventId)
            ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT UQ_Registrations_User_Event UNIQUE NONCLUSTERED (UserId, EventId)
    );
    -- UserId is already the leading key of the unique non-clustered index above.
    CREATE NONCLUSTERED INDEX IX_Registrations_EventId ON dbo.Registrations (EventId)
        INCLUDE (UserId, RegisteredAt);
END;

-- Available seats are derived, not stored. Cross-row capacity is enforced by
-- RegistrationService's locked transaction; CHECK cannot count related rows.
-- NO ACTION preserves attendance history and blocks accidental parent deletion.
COMMIT TRANSACTION;
`````

## database/seed.sql

`````sql
-- Fictional demonstration events, not official university announcements.
-- Only seed an empty catalog. Never modify existing event dates or registrations.
IF NOT EXISTS (SELECT 1 FROM dbo.Events)
BEGIN
    DECLARE @day DATETIMEOFFSET(0) = TODATETIMEOFFSET(
        CAST(CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(), '+08:00') AS DATE) AS DATETIME2), '+08:00');
    INSERT INTO dbo.Events (Title, Description, Location, StartsAt, Capacity) VALUES
    (N'Build Something Good', N'A hands-on campus hackathon. Bring an idea, meet a team, and build a small solution that makes student life better.', N'Innovation Lab', DATEADD(HOUR, 9, DATEADD(DAY, 3, @day)), 40),
    (N'Beyond the Classroom', N'An open conversation with alumni about first jobs, unexpected turns, and finding your own path in technology.', N'University Auditorium', DATEADD(HOUR, 14, DATEADD(DAY, 5, @day)), 120),
    (N'A Greener Tomorrow', N'Spend a morning growing something together. Join fellow students for a campus planting and sustainability workshop.', N'Campus Gardens', DATEADD(HOUR, 8, DATEADD(DAY, 7, @day)), 30),
    (N'Design for Everyone', N'Explore how thoughtful interfaces make everyday experiences more accessible. A beginner-friendly design workshop.', N'Computer Laboratory 2', DATEADD(HOUR, 13, DATEADD(DAY, 9, @day)), 25),
    (N'Campus in Color', N'A student-led showcase of illustration, photography, and creative work. Come for the art, stay for the conversations.', N'Student Center', DATEADD(HOUR, 10, DATEADD(DAY, 12, @day)), 80),
    (N'Find Your People', N'Meet student organizations and discover a new interest at this relaxed afternoon community gathering.', N'University Courtyard', DATEADD(HOUR, 15, DATEADD(DAY, 14, @day)), 100);
END;
`````

## docs/test-notes.md

`````markdown
# Task 4A — Validation and mock isolation

`backend/EmailValidator.cs` defines `IUniversityDomainPolicy` and `EmailValidator`. The production policy returns the configured university domain. Tests use a Moq object to supply that domain and verify whether it was consulted. No unit test connects to SQL Server, calls HTTP, reads real configuration, or accesses a university directory.

`tests/CampusEvents.Tests/EmailValidatorTests.cs` uses xUnit's Fact and Theory tests in Arrange–Act–Assert order. There are 27 executed cases: valid normal/case-insensitive/trimmed/plus-address emails; non-campus, suffix-spoofed, and subdomain addresses; null/empty/whitespace and malformed values; 64/65-character local-part boundaries and overlength input; a changed mocked domain; and invalid input rejected before the scalar C# refactor opens a connection.

The policy is consulted once for valid input and never for syntactically malformed input. This is the examination's required external-dependency mock. It deliberately mocks the small configuration boundary used by the real validator, rather than inventing a university-directory feature.

Separate integration checks in `tests/DatabaseVerification.cs` exercise actual HTTP routes and SQL constraints in a dedicated database. `tests/browser.mjs` exercises the real browser workflow and uses axe for automated accessibility checks. These are clearly separated from isolated unit tests.

The initial browser test assumed the desired event was first in the administrator's list. Integration fixtures disproved that assumption. The test now selects the event by its title, and screenshots are generated against a separate browser-verification database. This is an AI-made test refinement, not a fabricated human correction.
`````

## docs/security-review.md

`````markdown
# Task 4B — Security diagnosis

The exact flawed examination snippet is retained in `docs/PROMPTS.md`, Task 4B. Findings below are limited to behavior visible in that snippet.

| Issue | Evidence in code | Risk | Recommended fix |
|---|---|---|---|
| SQL injection | `"WHERE Email = '" + inputEmail + "'"` | User input becomes executable SQL; quotes and SQL syntax can change the query | Bind a typed `@Email` parameter and keep the SQL text fixed |
| Connection is not disposed | `SqlConnection conn = new SqlConnection(connStr); conn.Open();` without `using` or `finally` | Connections can remain checked out, exhausting the pool, including after exceptions | Place the connection in a `using` scope |
| Command is not disposed | `SqlCommand cmd = new SqlCommand(...)` without disposal | Command-owned resources are not deterministically released | Give the command its own `using` scope |
| Null result is dereferenced | `cmd.ExecuteScalar().ToString()` | An empty result can throw `NullReferenceException` | Check `null` and `DBNull` before conversion |
| Embedded connection credentials | Username and password are embedded in `connStr` | Real credentials substituted here could leak through source control and complicate configuration | Inject configuration; use integrated security locally and environment/secret configuration for deployment |
| Ambiguous scalar query | `SELECT *` combined with `ExecuteScalar()` and no ordering | Returns just the first column of one unspecified row, not a meaningful registration record | Select a specific registration ID with a deterministic order and describe the return contract |

First parameterize the query and guarantee disposal; these are the two mandated security corrections. Next handle empty results and make the scalar result explicit. Externalize deployment credentials. The snippet alone does not establish missing authentication, insecure transport, or a particular production configuration, so those are not asserted as findings.

## Task 4C — Refactor explanation

`backend/RegistrationService.cs`, method `GetUserRegistration`, contains the C# refactor. It uses `Microsoft.Data.SqlClient`, a fixed query with `@Email`, and an explicitly typed and sized `SqlDbType.NVarChar` parameter. SQL text and input travel separately: quote characters within an allowed email are data, not SQL syntax. Domain validation is an additional business rule, not the injection defense.

Both the connection and command use `using var`. They are disposed when the scope exits, including on exceptions; the command disposes before its connection. `ExecuteScalar()` is checked for both `null` and `DBNull`. The normalized schema stores email on Users, so the final query joins Users to Registrations instead of incorrectly assuming a Registrations.Email column exists. The return contract is the most recent registration ID as a string, or null if the user has no registrations.

The remainder of the service applies the same principles to async calls with `await using`, including readers and transactions. Uncommitted transactions are disposed on failure. Capacity enforcement locks the selected event before counting and inserting within a serializable transaction. The database's unique constraint remains a second layer of duplicate protection.

## Prototype limitations

The organizer key is a local demonstration guard. The application binds to loopback by default, does not store the key in browser storage, and does not contain a production password. Domain validation does not verify mailbox ownership, student enrollment, or identity. Use fictional students only; public deployment would require real authentication/authorization, HTTPS, and an appropriate privacy policy. Direct SQL writes can bypass the service's capacity rule; constrain database write privileges in a deployed system.

## Primary technical references

- [Microsoft: configuring SQL parameters](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/configuring-parameters-and-parameter-data-types)
- [Microsoft: C# using statement](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/using)
- [Microsoft: SQL Server transaction locking](https://learn.microsoft.com/en-us/sql/relational-databases/sql-server-transaction-locking-and-row-versioning-guide)
`````

## frontend/index.html

`````html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta
      name="description"
      content="Discover campus events, find your community, and save your seat."
    />
    <title>Campus Gather · Discover your next thing</title>
    <link rel="stylesheet" href="styles.css" />
    <script type="module" src="app.js"></script>
  </head>
  <body>
    <a class="skip-link" href="#main">Skip to content</a>
    <header class="site-header">
      <a class="brand" href="/" aria-label="Campus Gather home"
        ><span class="brand-mark" aria-hidden="true">cg.</span
        ><span>campus<span class="brand-light">gather</span></span></a
      >
      <nav aria-label="Main navigation">
        <a class="nav-active" href="#events" aria-current="page"
          >Discover events</a
        ><a href="admin.html"
          >For organizers <span aria-hidden="true">↗</span></a
        >
      </nav>
    </header>

    <main id="main">
      <section class="hero" aria-labelledby="hero-title">
        <div class="hero-copy">
          <p class="eyebrow">
            <span class="small-line" aria-hidden="true"></span> YOUR CAMPUS.
            YOUR COMMUNITY.
          </p>
          <h1 id="hero-title">
            Good things happen<br />when we <em>gather.</em>
          </h1>
          <p class="hero-description">
            New ideas, familiar faces, and something a little unexpected.<br
              class="desktop-break"
            />
            Find your next campus experience and make it yours.
          </p>
          <a class="button primary" href="#events"
            >Explore events <span aria-hidden="true">↗</span></a
          >
          <p class="hero-note">Made for the DLSU-D community.</p>
        </div>
        <div class="hero-art" aria-hidden="true">
          <div class="orbit orbit-one"></div>
          <div class="orbit orbit-two"></div>
          <span class="art-star">✳</span>
          <div class="art-ticket">
            <span>YOU'RE INVITED</span
            ><strong>A little curiosity.<br />A lot of possibility.</strong
            ><span class="ticket-bottom">CAMPUS GATHER <span>↗</span></span>
          </div>
          <span class="art-caption"
            >More than an event.<br />A place to belong.</span
          >
          <div class="art-circle"></div>
        </div>
      </section>

      <section id="events" class="catalog" aria-labelledby="events-title">
        <header class="section-heading">
          <div>
            <p class="eyebrow">MAKE ROOM FOR SOMETHING NEW</p>
            <h2 id="events-title">Coming up on campus</h2>
          </div>
          <p class="section-note">A seat for you. A story to take home.</p>
        </header>
        <div class="catalog-toolbar">
          <label class="search-label" for="event-search"
            ><span aria-hidden="true">⌕</span
            ><input
              id="event-search"
              type="search"
              aria-label="Search campus events"
              placeholder="Find an event, topic, or place"
              autocomplete="off"
          /></label>
          <p id="event-count" class="muted" role="status">Loading events…</p>
        </div>
        <p id="catalog-status" role="status" class="message">
          Finding your next campus experience…
        </p>
        <button id="retry-events" class="button secondary" type="button" hidden>
          Try again
        </button>
        <div
          id="event-grid"
          class="event-grid"
          aria-label="Upcoming campus events"
        ></div>
      </section>

      <section class="community-note" aria-labelledby="community-title">
        <span class="community-spark" aria-hidden="true">✳</span>
        <div>
          <h2 id="community-title">Your next connection starts here.</h2>
          <p>
            Show up curious. Leave with a new idea, a new friend, or a new
            favorite thing.
          </p>
        </div>
        <a href="#events" class="text-link"
          >Find your event <span aria-hidden="true">↗</span></a
        >
      </section>
    </main>

    <footer class="site-footer">
      <a class="brand footer-brand" href="/"
        >campus<span class="brand-light">gather</span></a
      >
      <p>A student-built campus event prototype. Sample events.</p>
      <span>DLSU-D COMMUNITY</span>
    </footer>

    <dialog id="registration-dialog" aria-labelledby="registration-title">
      <header class="dialog-header">
        <p class="eyebrow">SAVE YOUR SEAT</p>
        <button
          id="close-dialog"
          class="icon-button"
          type="button"
          aria-label="Close registration"
        >
          ×
        </button>
      </header>
      <h2 id="registration-title">Join the gathering</h2>
      <p id="selected-event-details" class="muted"></p>
      <form id="registration-form" novalidate>
        <p class="form-intro">
          All fields are required. Use your university email to register.
        </p>
        <label for="full-name">Full name</label>
        <input
          id="full-name"
          name="fullName"
          autocomplete="name"
          required
          minlength="2"
          maxlength="100"
          aria-describedby="name-error"
          placeholder="Your full name"
        />
        <p id="name-error" class="field-error"></p>
        <label for="email">University email</label>
        <input
          id="email"
          name="email"
          type="email"
          autocomplete="email"
          required
          maxlength="254"
          aria-describedby="email-hint email-error"
          placeholder="your.name@dlsud.edu.ph"
        />
        <p id="email-hint" class="field-hint">
          Use your @dlsud.edu.ph email address.
        </p>
        <p id="email-error" class="field-error"></p>
        <p id="form-error" class="message error" role="alert" hidden></p>
        <button
          id="submit-registration"
          class="button primary full-width"
          type="submit"
        >
          Confirm registration <span aria-hidden="true">↗</span>
        </button>
        <p class="privacy-note">
          Your name and email will be visible to the event organizer.
        </p>
      </form>
      <section
        id="registration-success"
        class="success-panel"
        aria-labelledby="success-title"
        hidden
      >
        <span class="success-mark" aria-hidden="true">✓</span>
        <h3 id="success-title" tabindex="-1">You're on the list!</h3>
        <p id="success-message"></p>
        <p id="confirmation-number" class="muted"></p>
        <button
          id="done-button"
          type="button"
          class="button primary full-width"
        >
          Back to exploring
        </button>
      </section>
    </dialog>
  </body>
</html>
`````

## frontend/styles.css

`````css
@charset "UTF-8";
:root {
  --paper: #f8f7f2;
  --surface: #fff;
  --ink: #203c32;
  --muted: #5d6962;
  --green: #245642;
  --green-hover: #173c2d;
  --line: #dedfd6;
  --input-border: #7c887f;
  --lime: #dfedb5;
  --error: #a32230;
  --error-bg: #fff0f0;
  --focus: #996400;
  --radius: 16px;
}
* {
  box-sizing: border-box;
}
html {
  scroll-behavior: smooth;
  scroll-padding-top: 24px;
}
body {
  margin: 0;
  background: var(--paper);
  color: var(--ink);
  font:
    16px/1.6 "Segoe UI",
    Arial,
    sans-serif;
}
button,
input,
select {
  font: inherit;
}
button,
a,
input,
select {
  -webkit-tap-highlight-color: transparent;
}
a {
  color: inherit;
}
button,
a {
  touch-action: manipulation;
}
button {
  cursor: pointer;
}
button:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}
button,
input,
select {
  min-height: 46px;
}
a:focus-visible,
button:focus-visible,
input:focus-visible,
select:focus-visible,
[tabindex="-1"]:focus-visible {
  outline: 3px solid var(--focus);
  outline-offset: 4px;
}
h1,
h2,
h3,
p {
  margin-top: 0;
}
h1,
h2,
h3 {
  line-height: 1.18;
}
h2 {
  font-size: 34px;
  letter-spacing: -1px;
}
h3 {
  font-size: 23px;
  letter-spacing: -0.45px;
}
[hidden] {
  display: none !important;
}
.muted {
  color: var(--muted);
}
.skip-link {
  position: absolute;
  top: -80px;
  left: 20px;
  background: var(--ink);
  color: white;
  padding: 12px 20px;
  z-index: 20;
}
.skip-link:focus {
  top: 12px;
}
.site-header,
.site-footer,
main {
  max-width: 1280px;
  margin: 0 auto;
}
.site-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 26px 40px;
  border-bottom: 1px solid var(--line);
  gap: 24px;
}
.brand {
  display: inline-flex;
  align-items: center;
  text-decoration: none;
  font-weight: 750;
  font-size: 24px;
  letter-spacing: -1px;
  gap: 11px;
}
.brand-light {
  font-weight: 400;
}
.brand-mark {
  display: grid;
  place-items: center;
  width: 42px;
  height: 42px;
  background: var(--green);
  color: var(--paper);
  border-radius: 50%;
  font-size: 23px;
  letter-spacing: -2px;
  padding-right: 3px;
}
.site-header nav {
  display: flex;
  align-items: center;
  gap: 34px;
}
.site-header nav a {
  text-decoration: none;
  font-size: 14px;
  padding: 11px 0;
}
.site-header nav a:hover {
  text-decoration: underline;
}
.nav-active {
  font-weight: 650;
}
.site-header nav a:last-child {
  color: var(--muted);
}
main {
  padding: 0 40px;
}
.hero {
  display: grid;
  grid-template-columns: 1.4fr 1fr;
  gap: 40px;
  align-items: center;
  padding: 76px 0 66px;
}
.eyebrow {
  font-size: 11px;
  font-weight: 750;
  letter-spacing: 1.8px;
  margin-bottom: 20px;
}
.small-line {
  display: inline-block;
  width: 20px;
  height: 2px;
  background: var(--green);
  vertical-align: middle;
  margin-right: 8px;
}
h1 {
  font-size: clamp(38px, 4.4vw, 62px);
  letter-spacing: -2.8px;
  font-weight: 550;
  margin-bottom: 23px;
}
h1 em {
  font-family: Georgia, serif;
  font-weight: 400;
  color: var(--green);
}
.hero-description {
  color: var(--muted);
  font-size: 15px;
  line-height: 1.8;
  margin-bottom: 27px;
}
.button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 30px;
  min-height: 48px;
  padding: 12px 23px;
  border: 1px solid transparent;
  border-radius: 7px;
  font-weight: 650;
  font-size: 14px;
  text-decoration: none;
  transition:
    background 0.18s,
    color 0.18s;
}
.primary {
  background: var(--green);
  color: white;
}
.primary:hover {
  background: var(--green-hover);
}
.secondary {
  background: transparent;
  border-color: var(--input-border);
  color: var(--green);
}
.secondary:hover {
  background: var(--lime);
}
.hero-note {
  font-size: 12px;
  color: var(--muted);
  margin: 14px 0 0;
}
.hero-art {
  height: 355px;
  position: relative;
  background: #e8eddd;
  border-radius: 48% 48% 10px 10px;
  overflow: hidden;
  margin: 0 4px 0 18px;
}
.art-ticket {
  position: absolute;
  left: 17%;
  top: 24%;
  width: 72%;
  padding: 24px 22px;
  background: var(--green);
  color: #f2f2df;
  transform: rotate(-8deg);
  border-radius: 8px;
  box-shadow: 8px 12px 0 #b6c5a7;
}
.art-ticket > span:first-child {
  font-size: 10px;
  letter-spacing: 2px;
}
.art-ticket strong {
  display: block;
  font:
    italic 29px/1.2 Georgia,
    serif;
  margin: 19px 0 24px;
}
.ticket-bottom {
  display: flex;
  justify-content: space-between;
  border-top: 1px dashed #a9bb9b;
  padding-top: 12px;
  font-size: 9px;
  letter-spacing: 2px;
}
.art-star {
  position: absolute;
  top: 13px;
  right: 30px;
  color: #ad562f;
  font-size: 93px;
  z-index: 1;
  line-height: 1;
}
.orbit {
  position: absolute;
  border: 1px solid #bbc5ac;
  border-radius: 50%;
  width: 340px;
  height: 340px;
  left: -150px;
  top: 170px;
}
.orbit-two {
  left: -170px;
  top: 187px;
}
.art-caption {
  position: absolute;
  bottom: 23px;
  left: 24px;
  font-size: 12px;
  line-height: 1.4;
}
.art-circle {
  position: absolute;
  bottom: -37px;
  right: -16px;
  width: 132px;
  height: 132px;
  border-radius: 50%;
  background: #d6e5a7;
}
.catalog {
  border-top: 1px solid var(--line);
  padding-top: 43px;
  padding-bottom: 50px;
}
.section-heading {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 24px;
}
.section-heading .eyebrow {
  margin-bottom: 12px;
}
.section-heading h2 {
  margin-bottom: 0;
}
.section-note {
  font-size: 13px;
  color: var(--muted);
  margin-bottom: 5px;
}
.catalog-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
  margin: 27px 0;
}
.search-label {
  position: relative;
  width: 350px;
  max-width: 100%;
}
.search-label span {
  position: absolute;
  left: 14px;
  top: 3px;
  font-size: 27px;
  color: var(--muted);
}
.search-label input {
  width: 100%;
  background: transparent;
  padding: 10px 14px 10px 44px;
  font-size: 13px;
}
.catalog-toolbar p {
  font-size: 12px;
  margin: 0;
}
.event-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 24px;
}
.event-card {
  background: var(--surface);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  transition: box-shadow 0.18s;
}
.event-card:hover {
  box-shadow: 0 7px 24px #203c320d;
}
.event-art {
  height: 168px;
  position: relative;
  overflow: hidden;
  background: #e6eadb;
  padding: 23px;
}
.event-art .art-word {
  position: absolute;
  bottom: 15px;
  left: 23px;
  font:
    italic 36px/1 Georgia,
    serif;
  letter-spacing: -1px;
  z-index: 1;
}
.event-art::before,
.event-art::after {
  content: "";
  position: absolute;
  width: 150px;
  height: 150px;
  border: 1px solid currentColor;
  border-radius: 50%;
  right: -12px;
  top: -29px;
  opacity: 0.3;
}
.event-art::after {
  right: 30px;
  top: 13px;
}
.tone-0 {
  background: #dce7cf;
  color: #315a3e;
}
.tone-1 {
  background: #e9dfef;
  color: #604368;
}
.tone-2 {
  background: #f1e6bf;
  color: #6b561e;
}
.tone-3 {
  background: #d9e6ec;
  color: #345361;
}
.tone-4 {
  background: #f0d9cb;
  color: #794c36;
}
.tone-5 {
  background: #e3e5dc;
  color: #4c5843;
}
.event-art .art-tag {
  font-size: 9px;
  letter-spacing: 2px;
  font-weight: 750;
}
.event-date-badge {
  position: absolute;
  right: 17px;
  bottom: 16px;
  background: #ffffffeb;
  color: var(--ink);
  width: 47px;
  padding: 6px;
  text-align: center;
  border-radius: 6px;
  z-index: 2;
}
.event-date-badge span {
  display: block;
  font-size: 9px;
  font-weight: 700;
  letter-spacing: 1px;
}
.event-date-badge strong {
  display: block;
  font-size: 20px;
  line-height: 1.1;
}
.event-content {
  padding: 23px;
  display: flex;
  flex: 1;
  flex-direction: column;
}
.event-meta {
  font-size: 11px;
  font-weight: 650;
  letter-spacing: 0.3px;
  color: var(--muted);
  margin-bottom: 12px;
}
.event-content h3 {
  font-size: 21px;
  margin-bottom: 10px;
}
.event-description {
  font-size: 13px;
  line-height: 1.7;
  color: var(--muted);
  margin-bottom: 17px;
}
.event-location {
  font-size: 12px;
  margin: auto 0 19px;
  color: var(--muted);
}
.event-card-bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  border-top: 1px solid var(--line);
  padding-top: 16px;
}
.seat-count {
  font-size: 11px;
  color: var(--green);
  display: flex;
  align-items: center;
  gap: 7px;
}
.seat-count::before {
  content: "";
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
}
.register-button {
  border: 0;
  padding: 10px 13px;
  background: #edf2e7;
  color: var(--green);
  font-size: 12px;
  font-weight: 700;
  border-radius: 6px;
  min-height: 44px;
}
.register-button:hover {
  background: var(--lime);
}
.community-note {
  display: flex;
  align-items: center;
  gap: 24px;
  margin: 9px 0 55px;
  padding: 30px 32px;
  background: #ebeddf;
  border-radius: 12px;
}
.community-spark {
  font-size: 52px;
  color: var(--green);
  line-height: 1;
}
.community-note h2 {
  font-size: 23px;
  margin-bottom: 7px;
  letter-spacing: -0.5px;
}
.community-note p {
  font-size: 13px;
  color: var(--muted);
  margin-bottom: 0;
}
.text-link {
  font-size: 13px;
  font-weight: 700;
  margin-left: auto;
  white-space: nowrap;
  padding: 12px 0;
  text-underline-offset: 5px;
}
.site-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  border-top: 1px solid var(--line);
  padding: 27px 40px 32px;
}
.footer-brand {
  font-size: 20px;
}
.site-footer p {
  font-size: 11px;
  color: var(--muted);
  margin: 0;
}
.site-footer > span {
  font-size: 9px;
  letter-spacing: 1.5px;
  color: var(--muted);
}
input,
select {
  display: block;
  border: 1px solid var(--input-border);
  border-radius: 7px;
  padding: 11px 13px;
  color: var(--ink);
  background: white;
  width: 100%;
  min-width: 0;
}
label {
  display: block;
  font-weight: 650;
  font-size: 14px;
  margin-bottom: 7px;
}
input::placeholder {
  color: var(--muted);
}
input[aria-invalid="true"] {
  border-color: var(--error);
}
dialog {
  width: 480px;
  max-width: calc(100% - 32px);
  max-height: calc(100dvh - 40px);
  overflow: auto;
  padding: 28px;
  border: 1px solid var(--line);
  border-radius: var(--radius);
  color: var(--ink);
  box-shadow: 0 24px 100px #203c322b;
}
dialog::backdrop {
  background: #142c24a6;
}
.dialog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}
.dialog-header p {
  margin-bottom: 0;
}
.icon-button {
  border: 1px solid var(--line);
  background: transparent;
  border-radius: 50%;
  min-width: 44px;
  min-height: 44px;
  font-size: 26px;
  line-height: 1;
  color: var(--ink);
}
dialog h2 {
  font-size: 29px;
  margin-bottom: 10px;
}
dialog > .muted {
  font-size: 13px;
}
.form-intro {
  font-size: 13px;
  padding: 16px 0 0;
  border-top: 1px solid var(--line);
}
.field-hint,
.field-error {
  font-size: 12px;
  margin: 6px 0 0;
}
.field-error {
  color: var(--error);
  min-height: 18px;
  margin-bottom: 14px;
}
.message {
  padding: 14px 18px;
  background: #edf2e7;
  border-radius: 7px;
  font-size: 14px;
}
.message.error {
  background: var(--error-bg);
  color: var(--error);
}
.full-width {
  width: 100%;
}
.privacy-note {
  font-size: 11px;
  color: var(--muted);
  text-align: center;
  margin: 12px 0 0;
}
.success-panel {
  text-align: center;
  padding: 20px 0 4px;
}
.success-mark {
  display: grid;
  place-items: center;
  margin: 0 auto 18px;
  width: 58px;
  height: 58px;
  border-radius: 50%;
  background: var(--lime);
  font-size: 27px;
}
.success-panel h3 {
  font-size: 27px;
}
.success-panel p {
  font-size: 14px;
}
.admin-main {
  padding-top: 48px;
  padding-bottom: 70px;
  min-height: 75vh;
}
.admin-intro {
  max-width: 700px;
}
.admin-intro h1 {
  font-size: 45px;
}
.admin-access {
  padding: 25px;
  background: var(--surface);
  border: 1px solid var(--line);
  border-radius: 12px;
  margin: 30px 0;
  max-width: 650px;
}
.inline-form {
  display: flex;
  gap: 12px;
  align-items: flex-end;
}
.inline-form > div {
  flex: 1;
  min-width: 0;
}
.admin-toolbar {
  display: flex;
  justify-content: space-between;
  gap: 20px;
  align-items: flex-end;
  margin: 24px 0;
}
.admin-toolbar > div {
  max-width: 550px;
  flex: 1;
}
.table-scroll {
  overflow-x: auto;
  background: white;
  border: 1px solid var(--line);
  border-radius: 12px;
}
table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}
caption {
  text-align: left;
  font-size: 14px;
  padding: 18px 20px;
  background: #edf2e7;
  font-weight: 650;
}
th,
td {
  padding: 15px 20px;
  border-bottom: 1px solid var(--line);
  font-size: 14px;
}
th {
  font-size: 12px;
  color: var(--muted);
}
td {
  overflow-wrap: anywhere;
}
tbody tr:last-child td {
  border-bottom: 0;
}
.admin-topline {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
}
@media (min-width: 1440px) {
  .hero {
    padding-top: 88px;
    padding-bottom: 78px;
  }
  .hero-art {
    height: 375px;
  }
  .event-art {
    height: 182px;
  }
}
@media (max-width: 1000px) {
  .hero {
    gap: 24px;
  }
  .hero-art {
    margin: 0;
    height: 330px;
  }
  .art-ticket {
    left: 13%;
    width: 77%;
    padding: 20px 16px;
  }
  .art-ticket strong {
    font-size: 25px;
  }
  .event-grid {
    gap: 18px;
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
  .section-note {
    display: none;
  }
  .desktop-break {
    display: none;
  }
  .site-footer > span {
    display: none;
  }
  .community-note {
    gap: 20px;
  }
  .community-note .text-link {
    white-space: normal;
    min-width: 90px;
  }
  .hero-description {
    font-size: 14px;
  }
}
@media (max-width: 680px) {
  .site-header {
    padding: 20px;
    gap: 12px;
  }
  .brand {
    font-size: 20px;
  }
  .brand-mark {
    width: 35px;
    height: 35px;
    font-size: 20px;
  }
  .site-header nav {
    gap: 17px;
  }
  .site-header nav a {
    font-size: 12px;
  }
  .site-header nav a:first-child {
    display: none;
  }
  main {
    padding: 0 20px;
  }
  .hero {
    grid-template-columns: 1fr;
    padding: 45px 0 35px;
    gap: 28px;
  }
  h1 {
    font-size: 43px;
    letter-spacing: -1.8px;
  }
  .hero-art {
    height: 240px;
    border-radius: 90px 90px 10px 10px;
    max-width: 420px;
    width: 100%;
    justify-self: center;
  }
  .art-ticket {
    top: 18%;
    left: 22%;
    width: 63%;
    padding: 17px 19px;
  }
  .art-ticket strong {
    font-size: 24px;
    margin: 13px 0 18px;
  }
  .art-star {
    font-size: 68px;
    right: 36px;
  }
  .art-caption {
    font-size: 10px;
    bottom: 17px;
    left: 17px;
  }
  .art-circle {
    width: 90px;
    height: 90px;
  }
  .hero-note {
    font-size: 11px;
  }
  .eyebrow {
    font-size: 9px;
    letter-spacing: 1.6px;
  }
  .catalog {
    padding-top: 30px;
  }
  .section-heading h2 {
    font-size: 29px;
  }
  .catalog-toolbar {
    flex-wrap: wrap;
    gap: 12px;
    margin-top: 22px;
  }
  .search-label {
    width: 100%;
    margin: 0;
  }
  .event-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  .event-art {
    height: 172px;
  }
  .event-content h3 {
    font-size: 22px;
  }
  .event-description {
    font-size: 14px;
  }
  .event-meta,
  .event-location,
  .register-button,
  .seat-count {
    font-size: 12px;
  }
  .community-note {
    padding: 23px;
    flex-wrap: wrap;
    gap: 15px;
    margin-bottom: 32px;
  }
  .community-spark {
    display: none;
  }
  .community-note h2 {
    font-size: 22px;
  }
  .community-note .text-link {
    margin: 0;
  }
  .site-footer {
    padding: 24px 20px;
    flex-wrap: wrap;
    gap: 10px;
  }
  .site-footer p {
    width: 100%;
  }
  .inline-form {
    flex-direction: column;
    align-items: stretch;
  }
  .admin-main {
    padding-top: 35px;
  }
  .admin-intro h1 {
    font-size: 36px;
  }
  .admin-toolbar {
    align-items: stretch;
    flex-direction: column;
  }
  .admin-access {
    padding: 20px;
  }
  .admin-topline {
    align-items: flex-start;
  }
  .admin-topline h2 {
    font-size: 27px;
  }
  th,
  td {
    padding: 13px 12px;
    font-size: 12px;
  }
  dialog {
    padding: 23px;
  }
  .table-scroll {
    width: 100%;
  }
}
@media (prefers-reduced-motion: reduce) {
  html {
    scroll-behavior: auto;
  }
  *,
  *::before,
  *::after {
    transition: none !important;
    animation: none !important;
  }
}
`````

## frontend/api.js

`````javascript
export async function request(path, options = {}) {
  let response;
  try {
    response = await fetch(path, {
      ...options,
      signal: AbortSignal.timeout(15000),
    });
  } catch {
    throw new Error(
      "We could not reach the server. Check your connection and try again.",
    );
  }
  const data = await response.json().catch(() => null);
  if (!response.ok)
    throw new Error(
      data?.message || "The request could not be completed. Please try again.",
    );
  return data;
}

export const dateFormat = new Intl.DateTimeFormat("en-PH", {
  timeZone: "Asia/Manila",
  month: "short",
  day: "numeric",
  year: "numeric",
});
export const timeFormat = new Intl.DateTimeFormat("en-PH", {
  timeZone: "Asia/Manila",
  hour: "numeric",
  minute: "2-digit",
});
export function eventDate(value) {
  return `${dateFormat.format(new Date(value))} · ${timeFormat.format(new Date(value))} PHT`;
}
export function element(tag, className, text) {
  const node = document.createElement(tag);
  if (className) node.className = className;
  if (text !== undefined) node.textContent = text;
  return node;
}
`````

## frontend/app.js

`````javascript
import { request, eventDate, element } from "./api.js";

const byId = (id) => document.getElementById(id);
const dialog = byId("registration-dialog");
const form = byId("registration-form");
const nameInput = byId("full-name");
const emailInput = byId("email");
let events = [];
let selectedEvent = null;
let universityDomain = "dlsud.edu.ph";
let submitting = false;
let trigger = null;

function renderEvents() {
  const query = byId("event-search").value.trim().toLowerCase();
  const matching = events.filter((event) =>
    `${event.title} ${event.description} ${event.location}`
      .toLowerCase()
      .includes(query),
  );
  byId("event-count").textContent =
    `${matching.length} upcoming event${matching.length === 1 ? "" : "s"} · Philippine time`;
  byId("event-grid").replaceChildren();
  byId("catalog-status").hidden = matching.length > 0;
  byId("catalog-status").textContent = events.length
    ? "No events match your search. Try another topic or place."
    : "No upcoming events just yet. Check back soon.";
  matching.forEach((event) => {
    const index = events.indexOf(event);
    const card = element("article", "event-card");
    const art = element("div", `event-art tone-${index % 6}`);
    art.setAttribute("aria-hidden", "true");
    art.append(
      element("span", "art-tag", "THE CAMPUS COLLECTION"),
      element(
        "span",
        "art-word",
        [
          "Create together.",
          "Go further.",
          "Grow with us.",
          "Think differently.",
          "Stay inspired.",
          "Find your people.",
        ][index % 6],
      ),
    );
    const badge = element("div", "event-date-badge");
    badge.append(
      element(
        "span",
        "",
        new Intl.DateTimeFormat("en", {
          month: "short",
          timeZone: "Asia/Manila",
        })
          .format(new Date(event.startsAt))
          .toUpperCase(),
      ),
      element(
        "strong",
        "",
        new Intl.DateTimeFormat("en", {
          day: "2-digit",
          timeZone: "Asia/Manila",
        }).format(new Date(event.startsAt)),
      ),
    );
    art.append(badge);
    const content = element("div", "event-content");
    const date = element("time", "event-meta", eventDate(event.startsAt));
    date.dateTime = event.startsAt;
    const heading = element("h3", "", event.title);
    const bottom = element("div", "event-card-bottom");
    const register = element(
      "button",
      "register-button",
      event.availableSeats > 0 ? "Save a seat ↗" : "Fully booked",
    );
    register.type = "button";
    register.disabled = event.availableSeats <= 0;
    register.setAttribute(
      "aria-label",
      event.availableSeats > 0
        ? `Register for ${event.title}`
        : `${event.title} is fully booked`,
    );
    register.addEventListener("click", () => openRegistration(event, register));
    bottom.append(
      element(
        "span",
        "seat-count",
        `${event.availableSeats} seat${event.availableSeats === 1 ? "" : "s"} left`,
      ),
      register,
    );
    content.append(
      date,
      heading,
      element("p", "event-description", event.description),
      element("p", "event-location", event.location),
      bottom,
    );
    card.append(art, content);
    byId("event-grid").append(card);
  });
}

async function loadEvents() {
  byId("retry-events").hidden = true;
  byId("catalog-status").hidden = false;
  byId("catalog-status").textContent = "Finding your next campus experience…";
  try {
    const [catalog, config] = await Promise.all([
      request("/api/events"),
      request("/api/config"),
    ]);
    events = catalog;
    universityDomain = config.universityDomain;
    byId("email-hint").textContent =
      `Use your @${universityDomain} email address.`;
    emailInput.placeholder = `your.name@${universityDomain}`;
    renderEvents();
  } catch (error) {
    byId("catalog-status").textContent = error.message;
    byId("event-count").textContent = "Events unavailable";
    byId("retry-events").hidden = false;
  }
}

function fieldError(input, message) {
  input.setAttribute("aria-invalid", String(Boolean(message)));
  byId(input === nameInput ? "name-error" : "email-error").textContent =
    message;
  return !message;
}

function validateName() {
  const value = nameInput.value.trim();
  return fieldError(
    nameInput,
    value.length < 2 || value.length > 100 || /[\x00-\x1f\x7f]/.test(value)
      ? "Enter your full name (2–100 characters)."
      : "",
  );
}

function validateEmail() {
  const value = emailInput.value.trim();
  const [local, domain, extra] = value.split("@");
  const valid =
    value.length <= 254 &&
    local &&
    local.length <= 64 &&
    /^[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+$/.test(local) &&
    !local.startsWith(".") &&
    !local.endsWith(".") &&
    !local.includes("..") &&
    domain?.toLowerCase() === universityDomain.toLowerCase() &&
    extra === undefined;
  return fieldError(
    emailInput,
    valid ? "" : `Enter a valid @${universityDomain} university email.`,
  );
}

function openRegistration(event, button) {
  selectedEvent = event;
  trigger = button;
  form.reset();
  fieldError(nameInput, "");
  fieldError(emailInput, "");
  byId("form-error").hidden = true;
  byId("registration-success").hidden = true;
  form.hidden = false;
  byId("registration-title").textContent = event.title;
  byId("selected-event-details").textContent =
    `${eventDate(event.startsAt)} · ${event.location}`;
  dialog.showModal();
  nameInput.focus();
}

function closeDialog() {
  if (!submitting) dialog.close();
}
byId("close-dialog").addEventListener("click", closeDialog);
byId("done-button").addEventListener("click", closeDialog);
dialog.addEventListener("cancel", (event) => {
  if (submitting) event.preventDefault();
});
dialog.addEventListener("close", () => {
  if (trigger?.isConnected) trigger.focus();
  else byId("event-search").focus();
});
nameInput.addEventListener("blur", validateName);
emailInput.addEventListener("blur", validateEmail);
byId("event-search").addEventListener("input", renderEvents);
byId("retry-events").addEventListener("click", loadEvents);

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  if (submitting) return;
  const nameValid = validateName();
  const emailValid = validateEmail();
  if (!nameValid || !emailValid) {
    (nameValid ? emailInput : nameInput).focus();
    return;
  }
  submitting = true;
  byId("submit-registration").disabled = true;
  byId("close-dialog").disabled = true;
  byId("submit-registration").textContent = "Saving your seat…";
  form.setAttribute("aria-busy", "true");
  byId("form-error").hidden = true;
  try {
    const result = await request("/api/registrations", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        eventId: selectedEvent.eventId,
        fullName: nameInput.value.trim(),
        email: emailInput.value.trim(),
      }),
    });
    form.hidden = true;
    byId("registration-success").hidden = false;
    byId("success-message").textContent =
      `Your seat at ${result.eventTitle} is confirmed. See you there!`;
    byId("confirmation-number").textContent =
      `Registration #${result.registrationId}`;
    byId("success-title").focus();
    await loadEvents();
  } catch (error) {
    byId("form-error").textContent = error.message;
    byId("form-error").hidden = false;
    await loadEvents();
  } finally {
    submitting = false;
    byId("submit-registration").disabled = false;
    byId("close-dialog").disabled = false;
    byId("submit-registration").textContent = "Confirm registration ↗";
    form.removeAttribute("aria-busy");
  }
});

loadEvents();
`````

## frontend/admin.html

`````html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Organizers · Campus Gather</title>
    <link rel="stylesheet" href="styles.css" />
    <script type="module" src="admin.js"></script>
  </head>
  <body>
    <a class="skip-link" href="#main">Skip to content</a>
    <header class="site-header">
      <a class="brand" href="/" aria-label="Campus Gather home"
        ><span class="brand-mark" aria-hidden="true">cg.</span
        ><span>campus<span class="brand-light">gather</span></span></a
      >
      <nav aria-label="Main navigation">
        <a href="/">Discover events</a
        ><a class="nav-active" href="admin.html" aria-current="page"
          >For organizers <span aria-hidden="true">↗</span></a
        >
      </nav>
    </header>
    <main id="main" class="admin-main">
      <section class="admin-intro" aria-labelledby="admin-title">
        <p class="eyebrow">BEHIND EVERY GOOD GATHERING</p>
        <h1 id="admin-title">Know who's coming.</h1>
        <p class="muted">
          Your event, your community. Find the registered attendees for each
          campus gathering.
        </p>
      </section>
      <section
        id="access-panel"
        class="admin-access"
        aria-labelledby="access-title"
      >
        <h2 id="access-title">Organizer access</h2>
        <p class="muted">
          Enter the access key provided by the person running this prototype.
        </p>
        <form id="access-form" class="inline-form">
          <div>
            <label for="admin-key">Administrator access key</label
            ><input
              id="admin-key"
              type="password"
              autocomplete="off"
              required
              aria-describedby="access-hint"
            />
          </div>
          <button id="unlock-button" class="button primary" type="submit">
            View attendees
          </button>
        </form>
        <p id="access-hint" class="field-hint">
          The key is kept in this page's memory only. Use sample student data
          for this classroom demo.
        </p>
      </section>
      <p id="admin-status" class="message" role="status" hidden></p>
      <section id="attendees-panel" aria-labelledby="attendees-title" hidden>
        <header class="admin-topline">
          <h2 id="attendees-title" tabindex="-1">The guest list</h2>
          <button id="lock-button" type="button" class="button secondary">
            Lock access
          </button>
        </header>
        <div class="admin-toolbar">
          <div>
            <label for="admin-event">Choose an event</label
            ><select id="admin-event"></select>
          </div>
          <button id="refresh-attendees" type="button" class="button secondary">
            Refresh list
          </button>
        </div>
        <div class="table-scroll">
          <table>
            <caption id="attendee-caption">
              Registered attendees
            </caption>
            <thead>
              <tr>
                <th scope="col">Full name</th>
                <th scope="col">University email</th>
                <th scope="col">Registered (PHT)</th>
              </tr>
            </thead>
            <tbody id="attendee-rows"></tbody>
          </table>
        </div>
        <p id="attendee-empty" class="message" hidden>
          No registrations yet. The first seat is waiting to be filled.
        </p>
      </section>
    </main>
    <footer class="site-footer">
      <a class="brand footer-brand" href="/"
        >campus<span class="brand-light">gather</span></a
      >
      <p>A student-built campus event prototype. Sample events.</p>
      <span>DLSU-D COMMUNITY</span>
    </footer>
  </body>
</html>
`````

## frontend/admin.js

`````javascript
import { request, eventDate, element } from "./api.js";
const byId = (id) => document.getElementById(id);
let accessKey = "";
let requestVersion = 0;

function status(message, isError = false) {
  byId("admin-status").textContent = message;
  byId("admin-status").hidden = !message;
  byId("admin-status").classList.toggle("error", isError);
}

async function loadAttendees() {
  const version = ++requestVersion;
  const eventId = byId("admin-event").value;
  byId("attendee-rows").replaceChildren();
  byId("attendee-empty").hidden = true;
  if (!eventId) {
    status("There are no events to display.");
    return;
  }
  status("Loading the guest list…");
  byId("refresh-attendees").disabled = true;
  try {
    const attendees = await request(`/api/admin/events/${eventId}/attendees`, {
      headers: { "X-Admin-Key": accessKey },
    });
    if (version !== requestVersion) return;
    attendees.forEach((attendee) => {
      const row = element("tr");
      row.append(
        element("td", "", attendee.fullName),
        element("td", "", attendee.email),
        element("td", "", eventDate(attendee.registeredAt)),
      );
      byId("attendee-rows").append(row);
    });
    const title = byId("admin-event").selectedOptions[0].textContent;
    byId("attendee-caption").textContent =
      `${title} · ${attendees.length} registered attendee${attendees.length === 1 ? "" : "s"}`;
    byId("attendee-empty").hidden = attendees.length > 0;
    status(
      `${attendees.length} attendee${attendees.length === 1 ? "" : "s"} loaded.`,
    );
  } catch (error) {
    if (version === requestVersion) status(error.message, true);
  } finally {
    if (version === requestVersion) byId("refresh-attendees").disabled = false;
  }
}

byId("access-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  accessKey = byId("admin-key").value;
  byId("unlock-button").disabled = true;
  status("Checking organizer access…");
  try {
    const events = await request("/api/admin/events", {
      headers: { "X-Admin-Key": accessKey },
    });
    byId("admin-event").replaceChildren(
      ...events.map((event) => {
        const option = element("option", "", event.title);
        option.value = event.eventId;
        return option;
      }),
    );
    byId("admin-key").value = "";
    byId("access-panel").hidden = true;
    byId("attendees-panel").hidden = false;
    byId("attendees-title").focus();
    await loadAttendees();
  } catch (error) {
    accessKey = "";
    status(error.message, true);
  } finally {
    byId("unlock-button").disabled = false;
  }
});
byId("admin-event").addEventListener("change", loadAttendees);
byId("refresh-attendees").addEventListener("click", loadAttendees);
byId("lock-button").addEventListener("click", () => {
  requestVersion++;
  accessKey = "";
  byId("admin-key").value = "";
  byId("attendee-rows").replaceChildren();
  byId("admin-event").replaceChildren();
  byId("attendees-panel").hidden = true;
  byId("access-panel").hidden = false;
  status("Organizer access is locked.");
  byId("admin-key").focus();
});
`````

## backend/RegistrationService.cs

`````csharp
using System.Data;
using Microsoft.Data.SqlClient;

namespace CampusEvents;

public sealed class RegistrationService(string connectionString, EmailValidator emailValidator)
{
    // Examination refactor: a deterministic scalar registration ID, or null if absent.
    // Email belongs to Users in the normalized schema, so use an explicit JOIN.
    public string? GetUserRegistration(string inputEmail)
    {
        if (!emailValidator.IsValid(inputEmail))
            throw new ArgumentException("A valid university email is required.", nameof(inputEmail));

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand("""
            SELECT TOP (1) r.RegistrationId
            FROM dbo.Registrations AS r
            INNER JOIN dbo.Users AS u ON u.UserId = r.UserId
            WHERE u.Email = @Email
            ORDER BY r.RegisteredAt DESC, r.RegistrationId DESC;
            """, connection);
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = inputEmail.Trim().ToLowerInvariant();
        connection.Open();
        var result = command.ExecuteScalar();
        return result is null or DBNull ? null : Convert.ToString(result, System.Globalization.CultureInfo.InvariantCulture);
    }

    public async Task<IReadOnlyList<EventSummary>> GetEventsAsync(bool includePast, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand("""
            SELECT e.EventId, e.Title, e.Description, e.Location, e.StartsAt, e.Capacity,
                   e.Capacity - COUNT(r.RegistrationId) AS AvailableSeats
            FROM dbo.Events AS e
            LEFT JOIN dbo.Registrations AS r ON r.EventId = e.EventId
            WHERE @IncludePast = 1 OR e.StartsAt > SYSDATETIMEOFFSET()
            GROUP BY e.EventId, e.Title, e.Description, e.Location, e.StartsAt, e.Capacity
            ORDER BY e.StartsAt, e.EventId;
            """, connection);
        command.Parameters.Add("@IncludePast", SqlDbType.Bit).Value = includePast;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var events = new List<EventSummary>();
        while (await reader.ReadAsync(cancellationToken))
            events.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3),
                reader.GetFieldValue<DateTimeOffset>(4), reader.GetInt32(5), reader.GetInt32(6)));
        return events;
    }

    public async Task<RegistrationConfirmation> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken)
    {
        var name = request.FullName?.Trim();
        if (name is null || name.Length is < 2 or > 100 || name.Any(char.IsControl))
            throw new RegistrationException(400, "Enter a full name between 2 and 100 characters.");
        if (!emailValidator.IsValid(request.Email))
            throw new RegistrationException(400, "Enter a valid university email address using the accepted domain.");
        if (request.EventId <= 0)
            throw new RegistrationException(400, "Choose an event before registering.");
        var email = request.Email!.Trim().ToLowerInvariant();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        string title;
        int capacity;
        // Serializes registration attempts for this event, including the last seat.
        await using (var eventCommand = new SqlCommand("""
            SELECT Title, Capacity, CASE WHEN StartsAt <= SYSDATETIMEOFFSET() THEN 1 ELSE 0 END AS HasStarted
            FROM dbo.Events WITH (UPDLOCK, HOLDLOCK) WHERE EventId = @EventId;
            """, connection, transaction))
        {
            eventCommand.Parameters.Add("@EventId", SqlDbType.Int).Value = request.EventId;
            await using var reader = await eventCommand.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
                throw new RegistrationException(404, "This event could not be found. Refresh the event catalog.");
            title = reader.GetString(0);
            capacity = reader.GetInt32(1);
            if (reader.GetInt32(2) == 1)
                throw new RegistrationException(409, "Registration has closed because this event has started.");
        }

        await using (var duplicateCommand = new SqlCommand("""
            SELECT COUNT(*) FROM dbo.Registrations AS r
            INNER JOIN dbo.Users AS u ON u.UserId = r.UserId
            WHERE r.EventId = @EventId AND u.Email = @Email;
            """, connection, transaction))
        {
            duplicateCommand.Parameters.Add("@EventId", SqlDbType.Int).Value = request.EventId;
            duplicateCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email;
            if (Convert.ToInt32(await duplicateCommand.ExecuteScalarAsync(cancellationToken)) > 0)
                throw new RegistrationException(409, "You are already registered for this event.");
        }

        await using (var countCommand = new SqlCommand(
            "SELECT COUNT(*) FROM dbo.Registrations WHERE EventId = @EventId;", connection, transaction))
        {
            countCommand.Parameters.Add("@EventId", SqlDbType.Int).Value = request.EventId;
            if (Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken)) >= capacity)
                throw new RegistrationException(409, "This event is full. Please choose another event.");
        }

        int userId;
        await using (var userCommand = new SqlCommand("""
            SELECT UserId, FullName FROM dbo.Users WITH (UPDLOCK, HOLDLOCK) WHERE Email = @Email;
            """, connection, transaction))
        {
            userCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email;
            await using var reader = await userCommand.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                userId = reader.GetInt32(0);
                if (!string.Equals(reader.GetString(1), name, StringComparison.OrdinalIgnoreCase))
                    throw new RegistrationException(409, "Use the same full name as your previous registration for this email.");
            }
            else userId = 0;
        }

        if (userId == 0)
        {
            await using var insertUser = new SqlCommand("""
                INSERT INTO dbo.Users (FullName, Email) OUTPUT INSERTED.UserId VALUES (@Name, @Email);
                """, connection, transaction);
            insertUser.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
            insertUser.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = email;
            userId = Convert.ToInt32(await insertUser.ExecuteScalarAsync(cancellationToken));
        }

        await using var insert = new SqlCommand("""
            INSERT INTO dbo.Registrations (UserId, EventId)
            OUTPUT INSERTED.RegistrationId VALUES (@UserId, @EventId);
            """, connection, transaction);
        insert.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
        insert.Parameters.Add("@EventId", SqlDbType.Int).Value = request.EventId;
        var registrationId = Convert.ToInt32(await insert.ExecuteScalarAsync(cancellationToken));
        await transaction.CommitAsync(cancellationToken);
        return new(registrationId, title, "You're on the list. Your registration is confirmed.");
    }

    public async Task<IReadOnlyList<Attendee>?> GetAttendeesAsync(int eventId, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var exists = new SqlCommand("SELECT COUNT(*) FROM dbo.Events WHERE EventId = @EventId;", connection);
        exists.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
        if (Convert.ToInt32(await exists.ExecuteScalarAsync(cancellationToken)) == 0) return null;
        await using var command = new SqlCommand("""
            SELECT r.RegistrationId, u.FullName, u.Email, r.RegisteredAt
            FROM dbo.Registrations AS r INNER JOIN dbo.Users AS u ON u.UserId = r.UserId
            WHERE r.EventId = @EventId ORDER BY r.RegisteredAt, r.RegistrationId;
            """, connection);
        command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var attendees = new List<Attendee>();
        while (await reader.ReadAsync(cancellationToken))
            attendees.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFieldValue<DateTimeOffset>(3)));
        return attendees;
    }
}
`````

## backend/EmailValidator.cs

`````csharp
using System.Text.RegularExpressions;

namespace CampusEvents;

public interface IUniversityDomainPolicy
{
    string GetAllowedDomain();
}

public sealed class UniversityDomainPolicy(string domain) : IUniversityDomainPolicy
{
    public string GetAllowedDomain() => domain;
}

public sealed partial class EmailValidator(IUniversityDomainPolicy policy)
{
    // Deliberately supports ordinary ASCII campus mailboxes, not all RFC 5322 syntax.
    public bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var value = email.Trim();
        if (value.Length > 254 || !CampusEmailPattern().IsMatch(value)) return false;
        var parts = value.Split('@');
        if (parts[0].Length > 64 || parts[0].StartsWith('.') || parts[0].EndsWith('.') || parts[0].Contains("..")) return false;
        return string.Equals(parts[1], policy.GetAllowedDomain(), StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex(@"\A[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+@[A-Za-z0-9.-]+\z", RegexOptions.CultureInvariant)]
    private static partial Regex CampusEmailPattern();
}
`````

## backend/Models.cs

`````csharp
namespace CampusEvents;

public sealed record EventSummary(int EventId, string Title, string Description, string Location,
    DateTimeOffset StartsAt, int Capacity, int AvailableSeats);
public sealed record RegistrationRequest(int EventId, string? FullName, string? Email);
public sealed record RegistrationConfirmation(int RegistrationId, string EventTitle, string Message);
public sealed record Attendee(int RegistrationId, string FullName, string Email, DateTimeOffset RegisteredAt);
public sealed class RegistrationException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
`````

## backend/Program.cs

`````csharp
using System.Security.Cryptography;
using System.Text;
using CampusEvents;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5080");
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 16 * 1024);
var connectionString = builder.Configuration.GetConnectionString("CampusEvents")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:CampusEvents.");
var universityDomain = builder.Configuration["UniversityDomain"] ?? "dlsud.edu.ph";
builder.Services.AddSingleton<IUniversityDomainPolicy>(new UniversityDomainPolicy(universityDomain));
builder.Services.AddSingleton<EmailValidator>();
builder.Services.AddScoped(provider => new RegistrationService(connectionString, provider.GetRequiredService<EmailValidator>()));
var app = builder.Build();

if (args.Contains("--init-db"))
{
    await DatabaseBootstrap.InitializeAsync(connectionString, seed: true);
    Console.WriteLine("Application database initialized. Existing records were preserved.");
    return;
}

// Single origin and loopback binding keep classroom setup straightforward.
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self'; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    if (context.Request.Path.StartsWithSegments("/api"))
        context.Response.Headers.CacheControl = "no-store";
    try { await next(context); }
    catch (RegistrationException exception)
    {
        context.Response.StatusCode = exception.StatusCode;
        await context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }
    catch (SqlException exception)
    {
        app.Logger.LogError("Database operation failed with SQL error {Number}.", exception.Number);
        context.Response.StatusCode = exception.Number is 2601 or 2627 ? 409 : 503;
        await context.Response.WriteAsJsonAsync(new { message = exception.Number is 2601 or 2627
            ? "This registration conflicts with an existing record. Refresh and check your details."
            : "The database is temporarily unavailable. Please try again shortly." });
    }
});

var files = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "wwwroot"));
app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = files });
app.UseStaticFiles(new StaticFileOptions { FileProvider = files });

app.MapGet("/api/config", () => Results.Ok(new { universityDomain }));
app.MapGet("/api/events", async (RegistrationService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetEventsAsync(false, cancellationToken)));
app.MapPost("/api/registrations", async (RegistrationRequest request, RegistrationService service, CancellationToken cancellationToken) =>
    Results.Json(await service.RegisterAsync(request, cancellationToken), statusCode: 201));

var admin = app.MapGroup("/api/admin");
admin.AddEndpointFilter(async (invocation, next) =>
{
    var key = app.Configuration["CAMPUS_ADMIN_KEY"];
    if (string.IsNullOrWhiteSpace(key))
        return Results.Json(new { message = "Administrator access is not configured. Set CAMPUS_ADMIN_KEY on the server." }, statusCode: 503);
    var supplied = invocation.HttpContext.Request.Headers["X-Admin-Key"].ToString();
    var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
    var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(supplied));
    if (!CryptographicOperations.FixedTimeEquals(expectedHash, suppliedHash))
        return Results.Json(new { message = "The administrator access key is incorrect." }, statusCode: 401);
    return await next(invocation);
});
admin.MapGet("/events", async (RegistrationService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetEventsAsync(true, cancellationToken)));
admin.MapGet("/events/{eventId:int}/attendees", async (int eventId, RegistrationService service, CancellationToken cancellationToken) =>
{
    var attendees = await service.GetAttendeesAsync(eventId, cancellationToken);
    return attendees is null ? Results.NotFound(new { message = "Event not found." }) : Results.Ok(attendees);
});

app.Run();
`````

## backend/DatabaseBootstrap.cs

`````csharp
using Microsoft.Data.SqlClient;

namespace CampusEvents;

public static class DatabaseBootstrap
{
    public static async Task InitializeAsync(string connectionString, bool seed)
    {
        var settings = new SqlConnectionStringBuilder(connectionString);
        var databaseName = settings.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName) || databaseName.Length > 128)
            throw new InvalidOperationException("Configure a dedicated database name.");
        if (new[] { "master", "model", "msdb", "tempdb" }.Contains(databaseName, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Use a dedicated application database, not a system database.");

        settings.InitialCatalog = "master";
        await using (var connection = new SqlConnection(settings.ConnectionString))
        {
            await connection.OpenAsync();
            // Identifiers cannot be SQL parameters: quote the configured identifier.
            var quotedName = "[" + databaseName.Replace("]", "]]", StringComparison.Ordinal) + "]";
            await using var command = new SqlCommand(
                $"IF DB_ID(@name) IS NULL CREATE DATABASE {quotedName};", connection);
            command.Parameters.Add("@name", System.Data.SqlDbType.NVarChar, 128).Value = databaseName;
            await command.ExecuteNonQueryAsync();
        }

        await using var database = new SqlConnection(connectionString);
        await database.OpenAsync();
        foreach (var file in seed ? new[] { "schema.sql", "seed.sql" } : new[] { "schema.sql" })
        {
            var sql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "database", file));
            await using var command = new SqlCommand(sql, database);
            await command.ExecuteNonQueryAsync();
        }
    }
}
`````

## tests/CampusEvents.Tests/EmailValidatorTests.cs

`````csharp
using CampusEvents;
using Moq;
using Xunit;

namespace CampusEvents.Tests;

public sealed class EmailValidatorTests
{
    private static (EmailValidator Validator, Mock<IUniversityDomainPolicy> Policy) CreateValidator()
    {
        var policy = new Mock<IUniversityDomainPolicy>(MockBehavior.Strict);
        policy.Setup(value => value.GetAllowedDomain()).Returns("dlsud.edu.ph");
        return (new EmailValidator(policy.Object), policy);
    }

    [Theory]
    [InlineData("student@dlsud.edu.ph")]
    [InlineData("john.alec@dlsud.edu.ph")]
    [InlineData("STUDENT@DLSUD.EDU.PH")]
    [InlineData("  student@dlsud.edu.ph  ")]
    [InlineData("student+club@dlsud.edu.ph")]
    public void Valid_campus_email_is_accepted(string email)
    {
        // Arrange
        var (validator, policy) = CreateValidator();
        // Act
        var result = validator.IsValid(email);
        // Assert: external configuration is supplied only by a mock.
        Assert.True(result);
        policy.Verify(value => value.GetAllowedDomain(), Times.Once);
    }

    [Theory]
    [InlineData("student@gmail.com")]
    [InlineData("student@dlsud.edu.ph.evil.test")]
    [InlineData("student@sub.dlsud.edu.ph")]
    [InlineData("student@fakedlsud.edu.ph")]
    public void Other_domains_are_rejected(string email)
    {
        var (validator, _) = CreateValidator();
        var result = validator.IsValid(email);
        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("student")]
    [InlineData("@dlsud.edu.ph")]
    [InlineData("student@@dlsud.edu.ph")]
    [InlineData("student name@dlsud.edu.ph")]
    [InlineData(".student@dlsud.edu.ph")]
    [InlineData("student.@dlsud.edu.ph")]
    [InlineData("student..name@dlsud.edu.ph")]
    [InlineData("Student <student@dlsud.edu.ph>")]
    [InlineData("student@dlsud.edu.ph\nInjected: header")]
    [InlineData("student\0@dlsud.edu.ph")]
    public void Malformed_input_is_rejected_before_consulting_policy(string? email)
    {
        var (validator, policy) = CreateValidator();
        var result = validator.IsValid(email);
        Assert.False(result);
        policy.Verify(value => value.GetAllowedDomain(), Times.Never);
    }

    [Theory]
    [InlineData(64, true)]
    [InlineData(65, false)]
    [InlineData(255, false)]
    public void Local_part_length_boundary_is_enforced(int length, bool expected)
    {
        var (validator, _) = CreateValidator();
        var result = validator.IsValid(new string('a', length) + "@dlsud.edu.ph");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void A_changed_external_domain_policy_is_mocked()
    {
        var policy = new Mock<IUniversityDomainPolicy>();
        policy.Setup(value => value.GetAllowedDomain()).Returns("example.edu");
        var validator = new EmailValidator(policy.Object);
        var result = validator.IsValid("student@example.edu");
        Assert.True(result);
        policy.Verify(value => value.GetAllowedDomain(), Times.Once);
    }

    [Fact]
    public void Invalid_refactor_input_never_attempts_a_database_connection()
    {
        var (validator, _) = CreateValidator();
        var service = new RegistrationService("not a connection string", validator);
        Assert.Throws<ArgumentException>(() => service.GetUserRegistration("' OR 1=1--"));
    }
}
`````

## tests/DatabaseVerification.cs

`````csharp
// Compiled by the separate integration project. No database in isolated unit tests.
using System.Data;
using System.Net;
using System.Net.Http.Json;
using CampusEvents;
using Microsoft.Data.SqlClient;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__CampusEvents")
    ?? throw new InvalidOperationException("Set ConnectionStrings__CampusEvents to a dedicated verification database.");
var databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
if (!databaseName.StartsWith("CampusEventsVerification_", StringComparison.Ordinal))
    throw new InvalidOperationException("This test only writes to databases named CampusEventsVerification_*.");
var adminKey = Environment.GetEnvironmentVariable("CAMPUS_ADMIN_KEY")
    ?? throw new InvalidOperationException("Set the test administrator key.");
using var client = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5080") };
await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
var passed = new List<string>();
void Check(bool condition, string label)
{
    if (!condition) throw new Exception("FAIL: " + label);
    passed.Add(label);
    Console.WriteLine("PASS: " + label);
}
async Task<int> Execute(string sql)
{
    await using var command = new SqlCommand(sql, connection);
    return await command.ExecuteNonQueryAsync();
}
async Task<int> Scalar(string sql)
{
    await using var command = new SqlCommand(sql, connection);
    return Convert.ToInt32(await command.ExecuteScalarAsync());
}
async Task ExpectConstraint(string sql, string label)
{
    try { await Execute(sql); throw new Exception("Expected a constraint violation: " + label); }
    catch (SqlException exception) when (exception.Number is 547 or 2601 or 2627) { Check(true, label); }
}

var events = await client.GetFromJsonAsync<List<EventSummary>>("/api/events") ?? [];
Check(events.Count >= 6, "Upcoming seeded catalog is served from SQL Server");
Check(events.All(value => value.StartsAt > DateTimeOffset.Now), "Catalog excludes started events");
var selectedEvent = events.Single(value => value.Title == "Build Something Good");
var eventId = selectedEvent.EventId;
var runId = Guid.NewGuid().ToString("N")[..12];
var email = $"integration.{runId}@dlsud.edu.ph";
var valid = new RegistrationRequest(eventId, "Integration Student", email);
using var registration = await client.PostAsJsonAsync("/api/registrations", valid);
Check(registration.StatusCode == HttpStatusCode.Created, "Valid registration returns HTTP 201");
using var duplicate = await client.PostAsJsonAsync("/api/registrations", valid with { Email = email.ToUpperInvariant() });
Check(duplicate.StatusCode == HttpStatusCode.Conflict, "Duplicate email/event is rejected case-insensitively");
using var invalid = await client.PostAsJsonAsync("/api/registrations", valid with { Email = "student@dlsud.edu.ph.attacker.test" });
Check(invalid.StatusCode == HttpStatusCode.BadRequest, "Spoofed domain suffix is rejected by server");
using var malformed = await client.PostAsJsonAsync("/api/registrations", valid with { FullName = " " });
Check(malformed.StatusCode == HttpStatusCode.BadRequest, "Invalid full name is rejected by server");
using var nonexistent = await client.PostAsJsonAsync("/api/registrations", valid with { EventId = int.MaxValue });
Check(nonexistent.StatusCode == HttpStatusCode.NotFound, "Nonexistent event returns HTTP 404");
using var anonymous = await client.GetAsync($"/api/admin/events/{eventId}/attendees");
Check(anonymous.StatusCode == HttpStatusCode.Unauthorized, "Attendees cannot be read without the admin key");
client.DefaultRequestHeaders.Add("X-Admin-Key", adminKey);
var attendees = await client.GetFromJsonAsync<List<Attendee>>($"/api/admin/events/{eventId}/attendees") ?? [];
Check(attendees.Any(value => value.Email == email), "Authorized administrator sees the committed attendee");
var updated = await client.GetFromJsonAsync<List<EventSummary>>("/api/events") ?? [];
Check(updated.Single(value => value.EventId == eventId).AvailableSeats == selectedEvent.AvailableSeats - 1, "Available seats are derived from committed registrations");
using var renamed = await client.PostAsJsonAsync("/api/registrations", valid with { EventId = events.Single(value => value.Title == "Beyond the Classroom").EventId, FullName = "Different Person" });
Check(renamed.StatusCode == HttpStatusCode.Conflict, "Existing user's name is not silently overwritten");

// Dedicated fixtures; retain for inspection in the verification database.
var lastSeatId = await Scalar("""
    INSERT INTO dbo.Events (Title,Description,Location,StartsAt,Capacity)
    OUTPUT INSERTED.EventId VALUES (N'Concurrency verification',N'Last-seat test',N'Test lab',DATEADD(DAY,2,SYSDATETIMEOFFSET()),1);
    """);
var attempts = Enumerable.Range(0, 8).Select(index => client.PostAsJsonAsync("/api/registrations",
    new RegistrationRequest(lastSeatId, $"Concurrent Student {index}", $"race.{runId}.{index}@dlsud.edu.ph")));
var responses = await Task.WhenAll(attempts);
Check(responses.Count(value => value.StatusCode == HttpStatusCode.Created) == 1, "Eight concurrent requests produce exactly one last-seat winner");
Check(responses.Count(value => value.StatusCode == HttpStatusCode.Conflict) == 7, "Other concurrent requests receive HTTP 409");
foreach (var response in responses) response.Dispose();
Check(await Scalar($"SELECT COUNT(*) FROM dbo.Registrations WHERE EventId = {lastSeatId}") == 1, "Last-seat event is not overbooked in the database");
var pastId = await Scalar("""
    INSERT INTO dbo.Events (Title,Description,Location,StartsAt,Capacity)
    OUTPUT INSERTED.EventId VALUES (N'Past verification',N'Closed event test',N'Test lab',DATEADD(DAY,-1,SYSDATETIMEOFFSET()),10);
    """);
using var past = await client.PostAsJsonAsync("/api/registrations", valid with { EventId = pastId });
Check(past.StatusCode == HttpStatusCode.Conflict, "Started events reject registration");
var afterPast = await client.GetFromJsonAsync<List<EventSummary>>("/api/events") ?? [];
Check(afterPast.All(value => value.EventId != pastId), "Started fixture is excluded from the student catalog");
var adminEvents = await client.GetFromJsonAsync<List<EventSummary>>("/api/admin/events") ?? [];
Check(adminEvents.Any(value => value.EventId == pastId), "Administrator can still select past events");

await ExpectConstraint("INSERT INTO dbo.Events (Title,Description,Location,StartsAt,Capacity) VALUES(N'Test',N'Test',N'Test',SYSDATETIMEOFFSET(),0)", "Capacity CHECK rejects zero");
await ExpectConstraint($"INSERT INTO dbo.Registrations(UserId,EventId) SELECT TOP(1) UserId,EventId FROM dbo.Registrations WHERE EventId={eventId}", "SQL UNIQUE rejects duplicate user/event pair");
await ExpectConstraint($"INSERT INTO dbo.Registrations(UserId,EventId) VALUES(2147483647,{eventId})", "SQL foreign key rejects missing user");
await ExpectConstraint($"DELETE FROM dbo.Events WHERE EventId={eventId}", "NO ACTION prevents deleting an event with attendance history");
Check(await Scalar("""
    SELECT COUNT(*) FROM sys.foreign_key_columns f
    WHERE NOT EXISTS (SELECT 1 FROM sys.index_columns ic JOIN sys.indexes i
      ON i.object_id=ic.object_id AND i.index_id=ic.index_id
      WHERE ic.object_id=f.parent_object_id AND ic.column_id=f.parent_column_id
        AND ic.key_ordinal=1 AND i.type=2)
    """) == 0, "Every foreign key is the leading column of a non-clustered index");
var service = new RegistrationService(connectionString, new EmailValidator(new UniversityDomainPolicy("dlsud.edu.ph")));
Check(service.GetUserRegistration(email) is not null, "Refactored scalar lookup returns a registration ID");
Check(service.GetUserRegistration($"missing.{runId}@dlsud.edu.ph") is null, "Refactored scalar lookup handles no rows without null dereference");
Check(service.GetUserRegistration($"x'--{runId}@dlsud.edu.ph") is null, "Valid-format SQL metacharacters are treated as data by the parameterized lookup");
await Execute($"UPDATE dbo.Events SET StartsAt=DATEADD(DAY,-1,SYSDATETIMEOFFSET()) WHERE EventId={lastSeatId}");
Console.WriteLine($"Integration verification passed: {passed.Count} checks. Database: {databaseName}");
`````

## tests/browser.mjs

`````javascript
import assert from "node:assert/strict";
import { mkdir, writeFile } from "node:fs/promises";
import { chromium } from "playwright";
import AxeBuilder from "@axe-core/playwright";

const adminKey = process.env.CAMPUS_ADMIN_KEY;
if (!adminKey)
  throw new Error(
    "Set CAMPUS_ADMIN_KEY to the running verification server key.",
  );
const output = "docs/evidence";
await mkdir(output, { recursive: true });
const browser = await chromium.launch({
  executablePath:
    process.env.CHROME_PATH ||
    "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
  headless: true,
});
const context = await browser.newContext({
  viewport: { width: 1440, height: 1080 },
  reducedMotion: "reduce",
});
const page = await context.newPage();
const failures = [];
const checks = [];
page.on("pageerror", (error) => failures.push(error.message));
const check = (condition, message) => {
  assert.ok(condition, message);
  checks.push(message);
  console.log(`PASS: ${message}`);
};
async function accessibility(label) {
  const report = await new AxeBuilder({ page })
    .withTags(["wcag2a", "wcag2aa", "wcag21a", "wcag21aa"])
    .analyze();
  await writeFile(
    `${output}/axe-${label}.json`,
    JSON.stringify(
      {
        url: report.url,
        violations: report.violations,
        passes: report.passes.map((item) => item.id),
        incomplete: report.incomplete.map((item) => ({
          id: item.id,
          description: item.description,
        })),
      },
      null,
      2,
    ),
  );
  check(
    report.violations.length === 0,
    `No automated WCAG A/AA violations: ${label} (${report.violations.map((item) => item.id).join(", ")})`,
  );
}

try {
  await page.goto("http://127.0.0.1:5080/");
  await page.locator(".event-card").first().waitFor();
  check(
    (await page.locator(".event-card").count()) >= 6,
    "Catalog renders real API event data",
  );
  await accessibility("catalog");
  await page.screenshot({
    path: `${output}/catalog-desktop.png`,
    fullPage: true,
  });

  for (const width of [375, 768, 1024, 1440]) {
    await page.setViewportSize({ width, height: 900 });
    check(
      await page.evaluate(
        () => document.documentElement.scrollWidth <= innerWidth,
      ),
      `No page overflow at ${width}px`,
    );
  }
  await page.setViewportSize({ width: 375, height: 812 });
  await page.screenshot({
    path: `${output}/catalog-mobile.png`,
    fullPage: true,
  });
  await page.getByRole("searchbox").fill("zz-no-such-event");
  check(
    (await page.locator(".event-card").count()) === 0,
    "Search displays a genuine empty state",
  );
  await page.getByRole("searchbox").fill("");
  await page.setViewportSize({ width: 1440, height: 1080 });
  await page.keyboard.press("Control+Home");
  await page
    .getByRole("button", {
      name: "Register for Build Something Good",
      exact: true,
    })
    .focus();
  await page.keyboard.press("Enter");
  await page.getByRole("dialog").waitFor();
  check(
    await page
      .getByLabel("Full name", { exact: true })
      .evaluate((node) => node === document.activeElement),
    "Opening registration moves keyboard focus into the form",
  );
  await accessibility("registration");
  await page.getByRole("button", { name: "Confirm registration" }).click();
  check(
    (await page.locator("#name-error").textContent()) !== "",
    "Empty form produces a textual validation error",
  );
  await page
    .getByLabel("Full name", { exact: true })
    .fill("Browser Demo Student");
  await page
    .getByLabel("University email", { exact: true })
    .fill("student@dlsud.edu.ph.evil.test");
  await page.getByRole("button", { name: "Confirm registration" }).click();
  check(
    (await page.locator("#email-error").textContent()) !== "",
    "Form rejects a deceptive university-domain suffix",
  );
  const email = `browser.${Date.now()}@dlsud.edu.ph`;
  await page.getByLabel("University email", { exact: true }).fill(email);
  await page.screenshot({
    path: `${output}/registration-form.png`,
    fullPage: true,
  });
  await page.getByRole("button", { name: "Confirm registration" }).click();
  await page.getByRole("heading", { name: "You're on the list!" }).waitFor();
  check(
    (await page.locator("#confirmation-number").textContent()) !== "",
    "Registration confirms a real server registration ID",
  );
  await accessibility("success");
  await page.screenshot({
    path: `${output}/registration-success.png`,
    fullPage: true,
  });
  await page.getByRole("button", { name: "Back to exploring" }).click();
  await page
    .getByRole("button", {
      name: "Register for Build Something Good",
      exact: true,
    })
    .click();
  await page
    .getByLabel("Full name", { exact: true })
    .fill("Browser Demo Student");
  await page.getByLabel("University email", { exact: true }).fill(email);
  await page.getByRole("button", { name: "Confirm registration" }).click();
  await page
    .getByRole("alert")
    .filter({ hasText: "already registered" })
    .waitFor();
  check(true, "Duplicate registration is shown as an actionable server error");
  await page.keyboard.press("Escape");
  await page.getByRole("dialog").waitFor({ state: "hidden" });
  check(true, "Escape closes the registration dialog");

  await page.goto("http://127.0.0.1:5080/admin.html");
  await accessibility("admin-locked");
  await page.getByLabel("Administrator access key").fill("incorrect-test-key");
  await page.getByRole("button", { name: "View attendees" }).click();
  await page
    .locator("#admin-status")
    .filter({ hasText: "incorrect" })
    .waitFor();
  check(
    await page.locator("#attendees-panel").isHidden(),
    "Wrong administrator key does not expose attendees",
  );
  await page.getByLabel("Administrator access key").fill(adminKey);
  await page.getByRole("button", { name: "View attendees" }).click();
  await page.locator("#attendees-panel").waitFor();
  await page
    .getByLabel("Choose an event")
    .selectOption({ label: "Build Something Good" });
  await page.getByRole("cell", { name: email, exact: true }).waitFor();
  check(true, "Organizer table shows the student registered through the UI");
  await accessibility("admin-attendees");
  await page.screenshot({
    path: `${output}/admin-attendees.png`,
    fullPage: true,
  });
  await page.setViewportSize({ width: 375, height: 812 });
  check(
    await page.evaluate(
      () => document.documentElement.scrollWidth <= innerWidth,
    ),
    "Administrator page fits mobile width",
  );
  await page.getByRole("button", { name: "Lock access" }).click();
  check(
    (await page.locator("#attendee-rows tr").count()) === 0,
    "Locking access clears attendee data from the page",
  );
  check(
    await page.evaluate(
      () => localStorage.length === 0 && sessionStorage.length === 0,
    ),
    "No credentials or attendee data are persisted in browser storage",
  );
  check(failures.length === 0, "No browser JavaScript exceptions");
  await writeFile(
    `${output}/browser-results.json`,
    JSON.stringify(
      {
        checkedAt: new Date().toISOString(),
        checks,
        pageErrors: failures,
        browser: await browser.version(),
        note: "Automated checks do not establish complete WCAG conformance or replace human review.",
      },
      null,
      2,
    ),
  );
  console.log(`Browser verification passed: ${checks.length} checks.`);
} finally {
  await browser.close();
}
`````
