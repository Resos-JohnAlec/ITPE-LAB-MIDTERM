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
