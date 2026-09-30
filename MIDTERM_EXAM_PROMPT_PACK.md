# Laboratory Midterm Examination - AI Prompt Pack

## Source Analysis

This prompt pack is derived from the uploaded **Laboratory Midterm Examination** for **Applied Generative AI for IT Solution Development**. The examination asks a 3-4 person team to build a working prototype for an **Online Campus Event Management System** in 3 hours, with a shared GitHub repository containing source code, database scripts, unit tests, and a consolidated `SUBMISSION.md` report.

### Core system requirements from the examination

The prototype must allow:

- Students to view upcoming campus events.
- Students to register for an event.
- Administrators to view registered attendees.

### Required repository deliverables

```text
/
├── frontend/
│   └── ... UI source code
├── backend/
│   └── RegistrationService.cs
├── database/
│   └── schema.sql
└── SUBMISSION.md
```

The examination specifically assigns these responsibilities:

| Member | Role | Primary Tasks |
|---|---|---|
| Member 1 | Systems Architect & Prompt Lead | Task 1 + Task 5 + integration |
| Member 2 | Frontend Engineer | Task 2 |
| Member 3 | Database & Backend Engineer | Task 3 |
| Member 4 | QA & Security Engineer | Task 4 |

For groups of 3, Task 4 is shared between Members 1 and 3.

---

# How to Use This File

Use the prompts below **one task at a time**. Do not ask the AI to complete the entire examination in one response. That makes it harder to verify the output and can create inconsistent deliverables.

For every task:

1. Paste the task prompt into your AI tool.
2. Keep the AI's response exactly as generated for the required `SUBMISSION.md` evidence.
3. Manually inspect the generated output before using it.
4. Apply corrections yourself where necessary.
5. Record important manual corrections in the Verification Log.

> **Important:** The examination requires the **exact prompt and AI output** for Task 1. For other tasks, keep a record of prompts and outputs as evidence so your team can accurately complete the AI Disclosure and Verification sections.

---

# MASTER PROJECT CONTEXT

Use this context at the beginning of prompts when your AI tool does not retain previous messages:

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

---

# TASK 1 - REQUIREMENTS ANALYSIS & PROMPT ARCHITECTURE

**Goal:** Produce the Production-Grade RCTC prompt required by the examination and use it to obtain an overall system design.

## Ready-to-Paste RCTC Prompt

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

## Task 1 Grounding Check

After receiving the AI output, manually check:

- Does it directly satisfy the three required system capabilities?
- Is the architecture actually buildable within 3 hours?
- Did the AI add unnecessary features or infrastructure?
- Are assumptions clearly identified?
- Does the recommended repository structure align with the examination deliverables?

### 3-4 Sentence Evaluation Template

```text
The AI-generated architecture is realistic for a 3-hour team prototype because it focuses on the required event catalog, event registration, and administrator attendee-viewing functions. The proposed components are separated into manageable frontend, backend, and database responsibilities that can be developed in parallel. Any advanced or optional features not required by the examination should be deferred to avoid overengineering. The team should still manually verify the technology choices, integration points, and assumptions before implementation.
```

---

# TASK 2 - AI-ASSISTED FRONTEND DEVELOPMENT

**Goal:** Generate the Event Catalog & Registration Form while satisfying the examination's semantic HTML5 and accessibility requirements.

## Ready-to-Paste Frontend Prompt

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

## Task 2 Manual Verification Checklist

Before committing the result into `/frontend`, inspect the generated UI and confirm:

```text
[ ] header exists
[ ] main exists
[ ] section/article elements are used appropriately
[ ] footer exists
[ ] each form field has a proper label
[ ] aria-label/accessible naming is present where needed
[ ] visible focus state exists
[ ] text contrast is readable
[ ] informative images have meaningful alt text
[ ] status/validation is not communicated by color alone
[ ] event catalog is visible
[ ] registration form is usable
[ ] no unnecessary features were introduced
```

---

# TASK 3 - DATABASE DESIGN & ERD GENERATION

**Goal:** Produce a 3NF relational design, Mermaid.js ERD, and `/database/schema.sql`.

## Ready-to-Paste Database Prompt

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

## Task 3 Manual Verification Checklist

```text
[ ] at least 3 relational entities exist
[ ] schema is in 3NF
[ ] Users, Events, Registrations are represented
[ ] PKs are defined
[ ] FKs are defined
[ ] duplicate user/event registration is prevented
[ ] meaningful CHECK constraints exist
[ ] foreign-key columns have non-clustered indexes
[ ] Mermaid ERD is present and syntactically plausible
[ ] SQL script is saved as /database/schema.sql
[ ] no unnecessary tables were added
```

---

# TASK 4 - SHIFT-LEFT TESTING, SECURITY & REFACTORING

## Part A - Unit Test Prompt

The examination asks for unit tests around a core validation routine, using Mock Objects to isolate external dependencies.

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

## Part B - Security Diagnosis Prompt

Use the exact flawed snippet from the examination in the prompt. Do not rewrite it before asking the AI to diagnose it.

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

## Part C - Refactoring Prompt

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

Save the final refactored implementation under:

```text
/backend/RegistrationService.cs
```

---

# TASK 5 - SUBMISSION.MD TEMPLATE

Create the root `SUBMISSION.md` using this structure. Replace every placeholder with the actual project information and exact AI evidence.

```markdown
# Online Campus Event Management System

## Team Roster

| Member | Role | Assigned Tasks |
|---|---|---|
| Member 1 Name | Systems Architect & Prompt Lead | Task 1, Task 5 |
| Member 2 Name | Frontend Engineer | Task 2 |
| Member 3 Name | Database & Backend Engineer | Task 3 |
| Member 4 Name | QA & Security Engineer | Task 4 |

## Project Overview

Briefly describe the prototype and its three required capabilities:
- View upcoming campus events.
- Register for an event.
- Allow administrators to view registered attendees.

## Setup Instructions

1. List prerequisites.
2. Explain how to install dependencies.
3. Explain how to configure the project, if needed.
4. Explain how to run the frontend.
5. Explain how to run the backend, if applicable.
6. Explain how to create the database using `database/schema.sql`.
7. Explain where the relevant source files are located.

## Task 1 - Requirements Analysis & Prompt Architecture

### Exact Prompt Used

> Paste the exact prompt sent to the AI here.

### AI Output

> Paste the exact AI output here.

### Manual Grounding Evaluation

Write 3-4 sentences evaluating whether the AI-generated architecture is realistic for a 3-hour team prototype.

## Task 2 - AI-Assisted Frontend Development

### AI Tool Used

Name/version of the tool used.

### Prompt Summary

Briefly describe the prompt used.

### Implementation Evidence

Describe the generated interface and the location of the implementation in `/frontend`.

### Accessibility Verification

| Requirement | Evidence / Manual Verification |
|---|---|
| Semantic HTML5 | |
| Proper labels | |
| aria-label / accessible naming | |
| Color contrast | |
| Image alt text | |
| Keyboard/focus accessibility | |
| Status not communicated by color alone | |

## Task 3 - Database Design & ERD Generation

### AI Tool Used

Name/version of the tool used.

### Prompt Summary

Briefly describe the database prompt used.

### Mermaid ERD

```mermaid
PASTE THE FINAL VERIFIED MERMAID ERD HERE
```

### Database Script

The final database DDL is located at:

`/database/schema.sql`

### Database Verification

Describe the manual checks performed for 3NF, foreign keys, CHECK constraints, uniqueness, and foreign-key indexes.

## Task 4 - Shift-Left Testing, Security & Refactoring

### Unit Testing

Describe the validation routine tested, framework used, test cases, and how mocks isolate external dependencies.

### Security Diagnosis

Summarize the identified SQL injection and resource-disposal problems from the flawed method.

### Refactored Backend

The final implementation is located at:

`/backend/RegistrationService.cs`

Describe the parameterized query and resource-disposal fixes.

## AI Disclosure Statement

List every AI tool used during the examination and explain:
- what the tool was used for;
- what type of output it generated;
- how the team manually verified the output before using it.

## Group Verification Log

| Task # | Identified AI Flaw / Limitation | Manual Correction Applied | Member Responsible |
|---|---|---|---|
| Task 1 | | | |
| Task 2 | | | |
| Task 3 | | | |
| Task 4 | | | |

## Final Repository Structure

```text
/
├── frontend/
├── backend/
│   └── RegistrationService.cs
├── database/
│   └── schema.sql
└── SUBMISSION.md
```

## Final Verification

- [ ] Project runs using the documented setup instructions.
- [ ] Frontend demonstrates the required event catalog and registration flow.
- [ ] Accessibility requirements were manually checked.
- [ ] Database script executes successfully in the selected DBMS.
- [ ] Mermaid ERD matches the final database schema.
- [ ] Unit tests run successfully.
- [ ] Security refactoring was manually reviewed.
- [ ] Verification Log contains at least 3 distinct manual corrections/refinements.
- [ ] AI Disclosure is complete.
- [ ] All required files are committed and pushed to GitHub.
```

---

# QUICK EXAM EXECUTION ORDER

Use this order to reduce integration problems:

```text
1. Task 1 -> lock the minimal architecture and stack decisions.
2. Task 3 -> create the schema and ERD before backend/data-dependent work.
3. Task 2 -> build the event catalog + registration form using the agreed structure.
4. Task 4 -> create tests, diagnose the flawed method, and produce RegistrationService.cs.
5. Task 5 -> integrate, manually verify, document corrections, and finalize SUBMISSION.md.
```

## 3-Hour Time Budget from the Examination

| Task | Examination Time | Points |
|---|---:|---:|
| Task 1 - Requirements & Prompt Architecture | 30 min | 20 |
| Task 2 - AI-Assisted Frontend | 45 min | 25 |
| Task 3 - Database Design & ERD | 45 min | 25 |
| Task 4 - Testing, Security & Refactoring | 45 min | 20 |
| Task 5 - Integration & Verification Report | 15 min | 10 |
| **Total** | **180 min** | **100** |

### Practical rule

Do not spend the entire time generating code. Reserve time for **manual verification, fixing AI mistakes, running the project, checking the SQL script, running tests, and completing `SUBMISSION.md`**, because the examination explicitly requires manual grounding and a verification log.

---

# AI RESPONSE QUALITY CHECK

Before accepting generated output from any task, ask:

```text
Does this directly satisfy the exam requirement?
Is it actually implementable in the remaining time?
Did the AI introduce unnecessary features?
Did the AI make assumptions without labeling them?
Does the generated code match the project's existing stack?
Can we explain and manually verify the output?
```

If an output fails one of these checks, correct it manually or send a focused follow-up prompt instead of regenerating the entire project.
