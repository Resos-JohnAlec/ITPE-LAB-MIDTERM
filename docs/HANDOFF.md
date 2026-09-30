# Handoff and remaining team actions

The working prototype, required source folders, schema, Mermaid ERD, mocked tests, C# security refactor, exact prompt documentation, generated-output evidence, screenshots, and consolidated SUBMISSION.md are present. The user-confirmed team is John Alec Resos, Renz Mathieu Raquin, and Matthew Ryan Sabino; registration accepts `@dlsud.edu.ph`.

## Verified locally

- .NET build: zero warnings, zero errors.
- Unit tests: 27 passing cases.
- SQL/API integration: 25 passing checks.
- Browser: 23 passing checks, including five axe scans with zero automated violations.
- Documented `Run-Local.ps1 -InitializeOnly` succeeds against the default demo database.
- Documented `Verify.ps1` completes using isolated databases and retains its evidence.

## Human examination evidence

Confirm the proposed task assignments in SUBMISSION.md. Each person should run the app, read the source for their task, and perform the manual grounding/accessibility/schema/security reviews. Record at least three actual student-made corrections or refinements, including the responsible member and date. The report already contains specific AI-made refinements to inspect, but they must not be relabeled as changes made by a student.

Update `docs/SUBMISSION.template.md` with the actual sign-offs and then run `node scripts/assemble-docs.mjs`. Preserve Task 1's exact prompt and original AI output. Add any other AI tools the team used independently.

## GitHub access blocker

The existing remote is `git@github-school:Resos-JohnAlec/ITPE-LAB-MIDTERM.git` on branch `main`. A read-only remote check outside the sandbox reached GitHub but failed with `Permission denied (publickey)`. A read-only HTTPS fallback to the same repository returned `Repository not found`, which can indicate unavailable/private repository access. No remote URL or SSH configuration was changed, and no successful push is claimed.

The repository owner/team needs to restore access for the configured GitHub account/key or provide the correct authorized remote. Then run `git push origin main` and verify the repository contains SUBMISSION.md, source, database scripts, tests, and evidence. Mark the GitHub publication checkbox complete only after that succeeds.

## Run the demonstration

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Run-Local.ps1
```

Open `http://127.0.0.1:5080`. The script prints a temporary organizer key for `/admin.html`. Use fictional names and university-format emails; no real student data is required.
