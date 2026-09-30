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
