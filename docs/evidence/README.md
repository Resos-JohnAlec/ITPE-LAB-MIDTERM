# Verification evidence

Executed on 30 September 2026 in this workspace using the project-local .NET 10 SDK, SQL Server LocalDB, Node.js, Playwright, and installed Chrome. These are actual agent-run results. No human review is implied.

| Evidence | Observed result |
|---|---|
| `unit-tests.trx` | 27 xUnit cases passed; zero failed/skipped; Moq isolates the university-domain policy |
| `integration-results.txt` | 25 live SQL/API checks passed, including constraints, duplicate prevention, time filtering, admin guard, null scalar lookup, SQL metacharacters, and last-seat concurrency |
| `browser-results.json` | 23 browser assertions passed; no JavaScript exceptions |
| `axe-catalog.json` | Zero automated violations in the student catalog |
| `axe-registration.json` | Zero automated violations in the registration form |
| `axe-success.json` | Zero automated violations in the confirmation view |
| `axe-admin-locked.json` | Zero automated violations in the organizer-access view |
| `axe-admin-attendees.json` | Zero automated violations in the attendee view |
| `catalog-desktop.png`, `catalog-mobile.png` | Actual rendered event catalog at desktop and mobile sizes |
| `registration-form.png`, `registration-success.png` | Actual filled form and server-backed confirmation |
| `admin-attendees.png` | Actual organizer table showing the browser test's fictional student |

The browser tested widths 375, 768, 1024, and 1440 pixels for overflow. The registration flow checks keyboard opening/focus, empty input, a deceptive domain suffix, a real success response, duplicate error, and Escape. Organizer checks cover a wrong key, the actual new attendee, locking access, and absence of credentials/attendee data in localStorage/sessionStorage.

The concurrency check sends eight requests for one seat, expects exactly one HTTP 201 and seven HTTP 409 responses, then confirms one database row. Integration and browser checks use separate dedicated databases. Test-created records are fictional, retained for inspection, and are not seeded into the default demonstration database. No database was dropped or existing user data deleted.

The reports retain axe's `incomplete` categories where a human judgment is needed. Zero automated violations does not mean full WCAG conformance. Human screen-reader use, complete contrast assessment, normalization/security review, and student-authored correction records remain pending in SUBMISSION.md.

To reproduce, stop any app using port 5080 and run:

```powershell
npm.cmd ci --ignore-scripts
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Verify.ps1
```

The script overwrites evidence with the new run's actual results and creates new `CampusEventsVerification_*` databases. Review reports before claiming completion. A failed run is not a passing result just because a report from an earlier run exists.
