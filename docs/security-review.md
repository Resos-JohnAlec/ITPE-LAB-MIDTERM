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
