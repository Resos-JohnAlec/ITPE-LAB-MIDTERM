# Prompt documentation

## Provenance

The prompts below are copied exactly from MIDTERM_EXAM_PROMPT_PACK.md and were adopted as task specifications while implementing this repository in one Codex session. They are not claimed to be separate messages pasted into an AI tool by students. The Task 1 architecture output is preserved verbatim in docs/task-1-ai-output.md and embedded in SUBMISSION.md. Other outputs are the generated project files, with a final snapshot in docs/AI_OUTPUTS.md. The original prompt pack remains unchanged.

## Actual user instructions in this session

1. "analyze the midterm_exam_propmt_pack.md and start with what's needed, after that, give me all prompts to document. make sure to accomplish deliverables."
2. "use @dlsud.edu.ph"
3. "for the names, its John Alec Resos, Renz Mathieu Raquin, and Matthew Ryan Sabino"

## Project-specific implementation context

The adopted task prompts were interpreted with the actual repository context: initially only README and the pack; a three-member roster; dlsud.edu.ph domain; SQL Server LocalDB installed; no available .NET SDK on PATH. A project-local .NET 10 SDK was installed to build and verify a single ASP.NET Core application, plain semantic frontend, and SQL Server schema. xUnit/Moq were selected for isolated tests. A local organizer access key guards the attendee API. These are disclosed implementation decisions, not quotations from a separately submitted prompt.

## Master project context

```text
PROJECT: Online Campus Event Management System

PROJECT GOAL:
Build a working prototype that allows students to view upcoming campus events, register for an event, and allows administrators to view registered attendees.

TIME LIMIT:
This is a 3-hour team prototype. Prefer simple, realistic, maintainable solutions that can actually be implemented within the time limit.

TEAM SIZE:
3-4 students.

DEVELOPMENT PRINCIPLE:
Prioritize the examination requirements over unnecessary features. Avoid overengineering. Clearly state assumptions instead of inventing unspecified requirements.

TRACEABILITY:
For every major design decision, explain which project requirement it satisfies.
```

## Task 1 — Exact RCTC architecture prompt

```text
ROLE
You are a Lead Systems Architect specializing in lightweight web application prototypes, requirements analysis, and practical architecture design.

CONTEXT
We are building a working prototype for an Online Campus Event Management System as a 3-hour team laboratory examination for 4th-year BSIT students.

The system must allow:
1. Students to view upcoming campus events.
2. Students to register for an event.
3. Administrators to view registered attendees.

The project will be developed collaboratively by a 3-4 person student team. The architecture must therefore be simple enough to implement, test, and integrate within the available time.

TASK
Create an overall system design for the Online Campus Event Management System.

Your response must include:
1. A concise requirements summary.
2. Functional requirements.
3. Non-functional requirements relevant to the prototype.
4. Recommended frontend, backend, database, and testing approach.
5. High-level system architecture and data flow.
6. Core modules/components and their responsibilities.
7. Main entities and relationships.
8. A practical API or service boundary proposal, if applicable.
9. A simple implementation plan ordered by priority.
10. Risks, assumptions, and time-saving decisions for a 3-hour prototype.
11. A suggested repository structure that separates frontend, backend, database, tests, and documentation.

CONSTRAINTS
- Keep the architecture realistic for a 3-hour team prototype.
- Prefer simple, maintainable solutions over enterprise-level complexity.
- Do not invent requirements that are not needed to satisfy the stated problem.
- Do not add microservices, event-driven infrastructure, Kubernetes, or other heavy infrastructure unless there is a direct requirement for them.
- Do not use third-party state management libraries such as Redux unless there is a clear project requirement for them.
- Do not introduce unnecessary authentication, payment, analytics, chat, or notification systems unless explicitly identified as assumptions for a future phase.
- Clearly label assumptions.
- Explain why each recommended technology or architectural decision is appropriate for the time limit.
- Keep the proposed solution implementable by beginner developers.

OUTPUT FORMAT
Use clear headings and concise tables where useful.
End with a short section titled "3-Hour Prototype Feasibility" explaining what should be implemented first and what should be deferred.
```

## Task 2 — Exact frontend prompt

```text
Act as a senior frontend engineer and accessibility-focused UI developer.

Build a prototype interface for an Online Campus Event Management System.

PRIMARY USER FLOW
A student should be able to:
1. View upcoming campus events.
2. Select an event.
3. Complete a registration form.
4. Submit the registration.

REQUIRED UI CONTENT
Create:
- An event catalog showing upcoming campus events.
- Event cards using realistic sample data.
- Event title, date/time, location, short description, and available seats.
- A registration form for the selected event.
- A clear submit action.
- Basic validation messages.
- A clear success state after a valid submission.

SEMANTIC HTML REQUIREMENT
Use semantic HTML5 elements such as:
<header>, <main>, <section>, <article>, and <footer>
where appropriate instead of using generic container elements for the main document structure.

WCAG / POUR ACCESSIBILITY REQUIREMENTS
Implement at minimum:
- Proper <label> elements for form controls.
- aria-label or equivalent accessible naming for inputs where needed.
- Visible keyboard-focus states.
- Sufficient color contrast between text and backgrounds.
- Meaningful alt text for informative images.
- Do not rely on color alone to communicate validation or status.
- Buttons and form controls must have clear accessible names.
- Use appropriate input types and validation attributes.

IMPLEMENTATION CONSTRAINTS
- Keep the UI simple enough for a 3-hour student prototype.
- Do not add unnecessary libraries or components.
- Do not add authentication, payment, chat, analytics, or other features outside the required scope.
- Do not use generic container wrappers when a semantic HTML5 element is appropriate.
- Use sample data only; do not pretend there is a real backend unless one is supplied.
- Keep code readable and easy for a student team to modify.
- Preserve the existing project framework and folder conventions if they are provided.

DELIVERABLE FORMAT
Return:
1. A concise explanation of the page structure.
2. The complete code needed for the prototype.
3. A short accessibility checklist mapping each implementation to the WCAG/POUR requirements above.
4. A short list of assumptions.
```

## Task 3 — Exact database prompt

```text
Act as a senior database architect responsible for a clean, production-oriented relational schema for a small campus event management prototype.

PROJECT
Online Campus Event Management System.

REQUIRED BUSINESS CAPABILITIES
- Students can view upcoming campus events.
- Students can register for an event.
- Administrators can view registered attendees.

MINIMUM DATA MODEL
Use at least these relational concepts/entities:
- Users
- Events
- Registrations

The design must be in Third Normal Form (3NF).

TASK
Design the relational database and produce all of the following:

1. Entity definitions with:
   - table name
   - purpose
   - columns
   - data types
   - primary keys
   - foreign keys
   - nullability
   - unique constraints where appropriate
   - business rules

2. A Mermaid.js Entity-Relationship Diagram using valid Mermaid ER syntax.

3. A complete SQL DDL script suitable for creating the schema.

SQL REQUIREMENTS
- Define primary keys.
- Define foreign keys explicitly.
- Define appropriate ON DELETE / ON UPDATE behavior and explain the choice.
- Add CHECK constraints for meaningful business rules.
- Add UNIQUE constraints where duplicate records should be prevented.
- Add non-clustered indexes on foreign-key columns.
- Add indexes only where they are useful for the required queries.
- Keep the schema normalized.
- Avoid storing repeated or derived data when it can be calculated or represented relationally.
- Use sensible naming conventions.
- Include comments for important constraints or design decisions.

REGISTRATION RULES
At minimum, the model should support:
- One user registering for an event.
- Tracking which user registered for which event.
- Preventing an accidental duplicate registration for the same user and event.
- Representing event capacity / available seats in a way that can be validated reliably.

TIME-LIMIT CONSTRAINT
This is a 3-hour team prototype. Do not overengineer the database with unnecessary tables or infrastructure.

OUTPUT ORDER
Return exactly these sections:
1. Design Assumptions
2. 3NF Table Design
3. Mermaid ERD
4. SQL DDL Script
5. Constraint and Index Summary
6. Short Verification Checklist

Before finalizing, self-check that every foreign key has an appropriate index and that the schema remains in 3NF.
```

## Task 4A — Exact unit-test prompt

```text
Act as a senior QA engineer and unit-testing specialist.

PROJECT
Online Campus Event Management System.

TASK
Create a small, testable validation routine relevant to event registration and then write unit tests for it using mock objects to isolate external dependencies.

PREFERRED EXAMPLE
Validate whether a student email belongs to the university domain and whether registration-related validation succeeds or fails under expected conditions.

TEST REQUIREMENTS
Include tests for:
1. Valid university email.
2. Invalid/non-university email.
3. Empty or null email input.
4. Boundary or malformed input that should be rejected.
5. A case where an external dependency is consulted and is mocked rather than accessed directly.

IMPLEMENTATION GUIDELINES
- Follow Arrange-Act-Assert.
- Keep each test focused on one behavior.
- Use mock objects to isolate external dependencies.
- Clearly identify what is being mocked and why.
- Avoid integration-test behavior inside the unit tests.
- Use the existing test framework in the project if one exists.
- If no framework has been selected, choose a common lightweight framework and state the assumption.

OUTPUT
Return:
1. The validation routine/interface needed by the tests.
2. The unit test code.
3. A brief explanation of each test case.
4. A short statement explaining how the mock isolates the external dependency.
```

## Task 4B — Exact security diagnosis prompt

```text
Act as a senior application security engineer reviewing C# database access code.

Analyze the following intentionally flawed method from an Online Campus Event Management System:

public string GetUserRegistration(string inputEmail) {
    string connStr = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
    SqlConnection conn = new SqlConnection(connStr);
    conn.Open();
    SqlCommand cmd = new SqlCommand("SELECT * FROM Registrations WHERE Email = '" + inputEmail + "'", conn);
    return cmd.ExecuteScalar().ToString();
}

TASK
Diagnose the code specifically for:
1. SQL injection vulnerability.
2. Unmanaged database connection/resource disposal.
3. Any related reliability or error-handling problems that are directly visible in the snippet.

For each issue:
- Identify the vulnerable line or pattern.
- Explain the risk.
- Explain the secure principle that should replace it.

Do not introduce unrelated security findings that cannot be supported by the code shown.

OUTPUT
Use a table with these columns:
Issue | Evidence in Code | Risk | Recommended Fix

Then provide a short prioritized remediation summary.
```

## Task 4C — Exact C# refactoring prompt

```text
Act as a senior C# backend engineer.

Refactor the following intentionally flawed method so that it is protected against SQL injection and properly disposes database resources:

public string GetUserRegistration(string inputEmail) {
    string connStr = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
    SqlConnection conn = new SqlConnection(connStr);
    conn.Open();
    SqlCommand cmd = new SqlCommand("SELECT * FROM Registrations WHERE Email = '" + inputEmail + "'", conn);
    return cmd.ExecuteScalar().ToString();
}

REQUIRED SECURITY FIXES
- Use a parameterized SQL query.
- Do not concatenate inputEmail into the SQL command.
- Use C# using statements / using declarations so the connection and command are disposed correctly.
- Preserve the original method's purpose as much as practical.
- Do not add unrelated frameworks or architecture changes.
- Handle the possibility that ExecuteScalar() returns null instead of calling ToString() blindly.
- Keep connection details out of production source in your recommended version where practical, but clearly separate that improvement from the two examination-mandated fixes.

OUTPUT
1. Refactored C# code.
2. Explanation of exactly how SQL injection is prevented.
3. Explanation of exactly how resource disposal is guaranteed.
4. A brief note about any additional defensive improvement you made.
```

## Pack quality-check prompt

```text
Does this directly satisfy the exam requirement?
Is it actually implementable in the remaining time?
Did the AI introduce unnecessary features?
Did the AI make assumptions without labeling them?
Does the generated code match the project's existing stack?
Can we explain and manually verify the output?
```

## Task 5 — Documentation instruction

The pack provides a report structure rather than a separate role/task prompt. The complete original structure is in its section "TASK 5 - SUBMISSION.MD TEMPLATE". It was applied to the actual implementation in root SUBMISSION.md. No student review, member correction, or GitHub publication is claimed without evidence.

## Optional future prompts (not used as user messages in this session)

These are ready to paste if the team needs further help. Keep future responses and actual human corrections in your own log.

### Human-grounding support

```text
Review SUBMISSION.md against MIDTERM_EXAM_PROMPT_PACK.md and the current source. Map every required deliverable to a file and execution result. Identify missing or unsupported claims. Do not invent manual verification, team contributions, model versions, or GitHub publication. Give the team a short checklist of concrete manual reviews still needed. Do not change code unless asked.
```

### Focused correction

```text
Review the failing behavior and evidence I provide for Campus Gather. Preserve the plain HTML/CSS/JavaScript frontend, ASP.NET Core C# backend, SQL Server 3NF schema, and exact dlsud.edu.ph domain. Explain the cause, apply the smallest necessary correction, and run the relevant existing checks. Record the actual change without representing AI edits as student-made manual corrections.
```

### Consolidation after team review

```text
Update docs/SUBMISSION.template.md using the actual team role confirmations, manual verification notes, and corrections I supply. Keep Task 1's exact adopted prompt and original AI output unchanged. Preserve the Mermaid diagram from database/erd.mmd. Regenerate SUBMISSION.md, docs/PROMPTS.md, and docs/AI_OUTPUTS.md with node scripts/assemble-docs.mjs. Mark checklist items complete only when supplied evidence supports them.
```
