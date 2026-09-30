# Task 4A — Validation and mock isolation

`backend/EmailValidator.cs` defines `IUniversityDomainPolicy` and `EmailValidator`. The production policy returns the configured university domain. Tests use a Moq object to supply that domain and verify whether it was consulted. No unit test connects to SQL Server, calls HTTP, reads real configuration, or accesses a university directory.

`tests/CampusEvents.Tests/EmailValidatorTests.cs` uses xUnit's Fact and Theory tests in Arrange–Act–Assert order. There are 27 executed cases: valid normal/case-insensitive/trimmed/plus-address emails; non-campus, suffix-spoofed, and subdomain addresses; null/empty/whitespace and malformed values; 64/65-character local-part boundaries and overlength input; a changed mocked domain; and invalid input rejected before the scalar C# refactor opens a connection.

The policy is consulted once for valid input and never for syntactically malformed input. This is the examination's required external-dependency mock. It deliberately mocks the small configuration boundary used by the real validator, rather than inventing a university-directory feature.

Separate integration checks in `tests/DatabaseVerification.cs` exercise actual HTTP routes and SQL constraints in a dedicated database. `tests/browser.mjs` exercises the real browser workflow and uses axe for automated accessibility checks. These are clearly separated from isolated unit tests.

The initial browser test assumed the desired event was first in the administrator's list. Integration fixtures disproved that assumption. The test now selects the event by its title, and screenshots are generated against a separate browser-verification database. This is an AI-made test refinement, not a fabricated human correction.
