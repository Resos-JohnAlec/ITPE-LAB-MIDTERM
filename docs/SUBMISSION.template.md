# Online Campus Event Management System

**Project:** Campus Gather · **Course:** Applied Generative AI for IT Solution Development · **Repository:** ITPE-LAB-MIDTERM

## Team Roster

The user confirmed these full names. Role allocation below is a proposed three-person split following the examination pack; actual human contributions and role acceptance still need team confirmation.

| Member | Proposed Role | Assigned Tasks |
|---|---|---|
| John Alec Resos | Systems Architect & Prompt Lead | Tasks 1 and 5, integration; share Task 4 |
| Renz Mathieu Raquin | Frontend Engineer | Task 2 |
| Matthew Ryan Sabino | Database & Backend Engineer | Task 3; share Task 4 |

## Project Overview

Campus Gather is a functioning local prototype. Students browse upcoming campus events and register using an `@dlsud.edu.ph` address. Organizers use a local administrator access key to view registered attendees for a selected event. The interface calls a C# API backed by SQL Server LocalDB; registration is persisted, duplicate sign-ups are blocked, and a locked transaction prevents overbooking through the service.

The three-member team, exact university domain, and names came from the user. Sample event names are fictional, not official DLSU-D announcements. The selected scope excludes full authentication, event editing, payments, notifications, and analytics. Domain validation is not proof of mailbox ownership or enrollment.

## Setup Instructions

1. Install the .NET 10 SDK and SQL Server LocalDB on Windows. Node.js and Chrome are optional prerequisites for browser checks. This workspace has a project-local SDK at `.tools/dotnet/dotnet.exe`, ignored by Git; it is not bundled for teammates.
2. From the repository root run `powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Run-Local.ps1`. It restores/builds the C# project, initializes a dedicated database, and starts the server. NuGet access is needed for the first restore.
3. `backend/appsettings.json` uses Windows integrated security for `(localdb)\MSSQLLocalDB`, database `CampusEventsMidterm`. Override it with environment variable `ConnectionStrings__CampusEvents` if needed. The configured university domain is `dlsud.edu.ph`.
4. Open `http://127.0.0.1:5080` for the frontend. The backend serves the frontend and API from the same origin; no separate UI server is required.
5. Open `/admin.html`, and enter the temporary administrator key printed by `Run-Local.ps1`. Alternatively set `CAMPUS_ADMIN_KEY` before starting. Never commit a real key. The browser retains it only in page memory.
6. `dotnet run --project backend/CampusEvents.csproj -- --init-db` creates the configured database, runs the exact `database/schema.sql`, then `database/seed.sql`. Re-running preserves existing data. Alternatively create/select a dedicated database in SSMS and run those two scripts in that order. Dates are seeded relative to initialization, with +08:00 offsets.
7. Run `dotnet test tests/CampusEvents.Tests/CampusEvents.Tests.csproj`. For complete verification with port 5080 free, run `npm.cmd ci --ignore-scripts` and `powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Verify.ps1`. Replace `dotnet` with `.\.tools\dotnet\dotnet.exe` when using the local SDK.
8. Source is in `frontend/`, `backend/`, `database/`, and `tests/`. Every adopted prompt is in [docs/PROMPTS.md](docs/PROMPTS.md), and generated-output evidence is in [docs/AI_OUTPUTS.md](docs/AI_OUTPUTS.md).

## Task 1 - Requirements Analysis & Prompt Architecture

### Exact Prompt Used

This is the RCTC prompt supplied in the user's prompt pack and adopted as the Task 1 specification in this Codex session. It was read from the file, not separately pasted by a team member into another AI chat. The actual user requests and implementation context are recorded in `docs/PROMPTS.md`.

```text
{{TASK1_PROMPT}}
```

### AI Output

The following reproduces `docs/task-1-ai-output.md` verbatim, including its headings. It is the architecture output authored in this session before the implementation.

{{TASK1_OUTPUT}}

### Manual Grounding Evaluation

**AI-drafted evaluation for human review; not a claim of completed student review:** The architecture directly supports event discovery, persistent registration, and organizer attendee viewing within one small application. Plain HTML/CSS/JavaScript and explicit SQL keep the components understandable, although restoring the SDK and setting up LocalDB can consume examination time. The database and C# requirements fit the selected stack, and the transaction addresses capacity without adding unrelated infrastructure. The team should confirm the setup, API integration, accessibility, and stated assumptions before adopting this evaluation as its own manual assessment.

## Task 2 - AI-Assisted Frontend Development

### AI Tool Used

OpenAI Codex in this workspace session. The exact serving model/build identifier is not independently verified; no version is invented. The local UI/UX Pro Max skill guided design and accessibility checks.

### Prompt Summary

Adopted the pack's frontend prompt to build semantic event cards and a registration form with labels, validation, focus, contrast, and success feedback. The project-specific context supplied the C# API, the exact university domain, and a simple organizer attendee page. The exact frontend prompt is preserved in `docs/PROMPTS.md`.

### Implementation Evidence

`frontend/index.html`, `styles.css`, `app.js`, and `api.js` implement the catalog and native registration dialog. `frontend/admin.html` and `admin.js` implement the attendee table and local organizer-key flow. The browser reads actual seeded database events; it does not fake registration success. Desktop, mobile, form, success, and administrator screenshots are in [docs/evidence](docs/evidence/).

### Accessibility Verification

| Requirement | Implementation / verification evidence |
|---|---|
| Semantic HTML5 | header, nav, main, section, article, footer, form, dialog, table, caption, time; axe landmark checks |
| Proper labels | Explicit full-name, email, administrator-key, event-select labels; search input has accessible name |
| aria-label / accessible naming | Named navigation, search, event-specific register buttons, close button, titled dialog; aria-describedby and aria-invalid |
| Color contrast | Semantic dark-green/cream palette; axe contrast scan in five UI states; complete manual contrast review remains for the team |
| Image alt text | No informative images; CSS/text decoration is aria-hidden; event details remain visible as ordinary text |
| Keyboard/focus accessibility | Skip link; visible focus outline; keyboard-opened native dialog; focus moves to the form; Escape closes it; focus restores after closing |
| Status not communicated by color alone | Text errors and confirmation, loading/empty states, role=status/alert; full events have text and disabled buttons |

Automated accessibility results are evidence, not a claim of full WCAG compliance. A human keyboard and screen-reader review remains required. The detailed POUR mapping is [docs/frontend-notes.md](docs/frontend-notes.md).

## Task 3 - Database Design & ERD Generation

### AI Tool Used

OpenAI Codex in this session; see disclosure above.

### Prompt Summary

Adopted the database prompt requiring Users, Events, Registrations, 3NF, explicit keys and referential actions, useful CHECK/UNIQUE constraints, Mermaid ERD, and non-clustered foreign-key indexes. SQL Server was selected to match the examination's C# snippet and indexing language.

### Mermaid ERD

```mermaid
{{ERD}}
```

### Database Script

The final SQL DDL is [database/schema.sql](database/schema.sql). Entity definitions and types, assumptions, normalization analysis, and index rationale are in [database/DESIGN.md](database/DESIGN.md). Fictional data is in `database/seed.sql`.

### Database Verification

The schema executed successfully in SQL Server LocalDB. Integration checks verified positive capacity, foreign-key rejection, composite registration uniqueness, restrictive parent deletion, and a leading non-clustered index for each FK. The composite `(UserId, EventId)` unique index covers UserId; a separate non-clustered EventId index covers event-attendee and capacity queries. The schema keeps user names/emails out of Registrations and derives available seats instead of storing redundant counts. The ERD is mechanically embedded from the canonical source; human normalization and cardinality sign-off remains pending.

## Task 4 - Shift-Left Testing, Security & Refactoring

### Unit Testing

`EmailValidator` checks syntax, maximum length, local-part boundaries, and an exact case-insensitive university domain. `IUniversityDomainPolicy` is an external configuration boundary mocked with Moq. xUnit executes 27 cases covering valid university addresses, non-university addresses, null/empty input, malformed or boundary input, deceptive suffixes/subdomains, and verification of mock calls. Unit tests do not access SQL Server or network services. Arrange–Act–Assert structure and case explanations are documented in [docs/test-notes.md](docs/test-notes.md).

**Observed result:** 27 passed, zero failed, zero skipped. Raw evidence: [unit-tests.trx](docs/evidence/unit-tests.trx). Separate live checks passed all 25 database/API assertions, including eight simultaneous last-seat requests yielding one HTTP 201 and seven HTTP 409 results. Raw evidence: [integration-results.txt](docs/evidence/integration-results.txt). Browser results: [browser-results.json](docs/evidence/browser-results.json).

### Security Diagnosis

| Issue | Evidence in flawed code | Risk | Recommended fix |
|---|---|---|---|
| SQL injection | `inputEmail` concatenated into command text | Input can alter SQL syntax | Typed `@Email` parameter |
| Connection not disposed | Opened connection has no `using`/`finally` | Pool/resource exhaustion after normal or exceptional exit | Scoped `using` connection |
| Command not disposed | Command has no disposal scope | Resources are not deterministically released | Scoped `using` command |
| Null dereference | `ExecuteScalar().ToString()` | Empty query result can throw | Handle `null` and `DBNull` |
| Embedded credentials | Connection string includes username/password fields | Real substituted secrets could leak | Configuration/integrated security |
| Ambiguous scalar result | `SELECT *` without ordering | Unspecified column/row returned | Select one documented ID with deterministic ordering |

The exact flawed snippet is preserved unchanged in the Task 4B and 4C prompts. Detailed diagnosis and prioritized remediation are in [docs/security-review.md](docs/security-review.md).

### Refactored Backend

The required implementation is [backend/RegistrationService.cs](backend/RegistrationService.cs), method `GetUserRegistration`. SQL is fixed and the email is a typed, sized parameter. Connection and command use `using var`, guaranteeing disposal on both success and exception. The normalized schema requires joining Users to Registrations. The method returns the latest registration ID or null when no record exists. Integration verification exercised a normal lookup, a no-row lookup, and a valid-format email containing SQL metacharacters. Input validation supports business rules; parameterization is the SQL injection defense.

## AI Disclosure Statement

| Tool / resource | Use and generated output | Actual verification status |
|---|---|---|
| OpenAI Codex | Architecture, HTML/CSS/JS UI, C# API and refactor, SQL, tests, documentation, and test corrections | The agent compiled and ran automated checks and inspected generated screenshots. Team manual review remains pending. |
| Local UI/UX Pro Max skill | Design and accessibility guidance for forms, focus, contrast, responsive layout, and feedback | Guidance was adapted to the exam scope; no added UI framework or runtime dependency |
| Microsoft documentation | Reference for .NET, Minimal APIs, SQL parameters, disposal, and locking | Technical references, not a separate generative AI tool |
| xUnit/Moq, Playwright/axe | Test execution, mock isolation, browser automation, accessibility reports | These are testing tools, not additional AI authors |

No other AI tool or image generator was used in this session. Team members must add any AI tools they independently used. The exact supplied prompts and actual user steering appear in [docs/PROMPTS.md](docs/PROMPTS.md); generated code/output snapshots appear in [docs/AI_OUTPUTS.md](docs/AI_OUTPUTS.md). Prompts are not represented as separate human-submitted conversations, and agent refinements are not represented as student-made changes.

## Group Verification Log

The examination requires at least three actual manual corrections/refinements with responsible team members. The AI cannot truthfully certify student work that has not happened. The implemented refinements below are review-ready; each team member must inspect them and record any correction they actually perform, along with their name/date. Until then, this is an agent verification record, not a completed human correction log.

| Task # | Identified AI Flaw / Limitation | Correction / Refinement Applied | Member Responsible |
|---|---|---|---|
| 1 | The project started without a selected stack; unconstrained choices could conflict with the C# and SQL Server deliverables | Selected one ASP.NET Core host, LocalDB, and framework-free frontend; deferred unrelated features | Agent implementation; proposed reviewer John Alec Resos, human sign-off pending |
| 2 | A search placeholder does not provide an explicit stable accessible name | Added `aria-label="Search campus events"`; native labels, focus flow, and textual validation accompany it | Agent correction; proposed reviewer Renz Mathieu Raquin, human sign-off pending |
| 3 | Persisting available seats could drift; checking capacity outside a transaction can overbook | Derived availability from registration count; locked event, count, and insert in one serializable transaction; verified concurrent last-seat requests | Agent refinement; proposed reviewer Matthew Ryan Sabino, human sign-off pending |
| 4 | The supplied snippet concatenates SQL input, leaks resources, assumes a Registrations.Email column, and dereferences empty scalar results | Parameterized query, `using` disposal, normalized JOIN, deterministic scalar ID, null handling | Agent refactor of the examination flaw; proposed reviewers John Alec Resos / Matthew Ryan Sabino, human sign-off pending |
| 4 | Initial browser test assumed the intended event was the first admin option; a concurrency fixture changed that order | Select the event explicitly by title and use a separate browser test database | Agent test correction; proposed QA reviewers John Alec Resos / Matthew Ryan Sabino, human sign-off pending |

## Final Repository Structure

```text
/
|-- frontend/                    Student and organizer UI
|-- backend/
|   |-- RegistrationService.cs  Required parameterized C# refactor + service
|   |-- Program.cs              API and static-file host
|   |-- EmailValidator.cs       Testable validation + domain policy
|   |-- DatabaseBootstrap.cs    Schema/seed execution
|   `-- CampusEvents.csproj
|-- database/
|   |-- schema.sql              Required 3NF DDL
|   |-- seed.sql
|   |-- erd.mmd
|   `-- DESIGN.md
|-- tests/
|   |-- CampusEvents.Tests/     xUnit + Moq
|   |-- CampusEvents.Integration/
|   |-- DatabaseVerification.cs
|   `-- browser.mjs
|-- scripts/                    Run, verify, assemble evidence
|-- docs/
|   |-- PROMPTS.md              All prompt documentation
|   |-- AI_OUTPUTS.md           Generated code/output snapshot
|   |-- task-1-ai-output.md     Original architecture output
|   `-- evidence/              Test reports and screenshots
|-- README.md
|-- MIDTERM_EXAM_PROMPT_PACK.md  Original user-supplied pack, preserved
`-- SUBMISSION.md
```

## Final Verification

- [x] Project builds successfully with zero warnings and errors.
- [x] Frontend demonstrates the actual event catalog, persisted registration, and attendee flow.
- [x] Automated accessibility scans, keyboard interactions, and responsive-width checks executed; see evidence.
- [ ] Team manually reviewed accessibility, including keyboard and screen-reader behavior.
- [x] Database script executes successfully in SQL Server LocalDB.
- [x] Mermaid ERD describes the final schema and is embedded from the canonical file.
- [x] 27 isolated unit tests pass.
- [x] 25 separate API/database checks pass, including capacity concurrency and SQL metacharacter handling.
- [ ] Team manually reviewed the security refactor and normalization design.
- [ ] Group log contains at least three actual student-made corrections/refinements with names and dates.
- [x] AI disclosure documents tools and actual verification limits for this session.
- [ ] Team confirms proposed role assignments and adds any independently used tools.
- [ ] All required files committed and pushed to GitHub; publication is blocked by the configured SSH key being rejected and the HTTPS fallback lacking repository access. See `docs/HANDOFF.md`.

## Submission Handoff

The implementation, prompt documentation, AI evidence, tests, and report are prepared. Human review and correction records must be completed by the team; they have deliberately not been invented. GitHub access must be restored before publication can be verified; details and commands are in [docs/HANDOFF.md](docs/HANDOFF.md). Re-run the documented verification when changing source files. `scripts/assemble-docs.mjs` regenerates this report and prompt/output documents from the preserved source pack, original architecture output, schema diagram, and this template.
