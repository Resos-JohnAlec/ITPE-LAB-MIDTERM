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
