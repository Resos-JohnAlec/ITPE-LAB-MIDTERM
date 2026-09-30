namespace CampusEvents;

public sealed record EventSummary(int EventId, string Title, string Description, string Location,
    DateTimeOffset StartsAt, int Capacity, int AvailableSeats);
public sealed record RegistrationRequest(int EventId, string? FullName, string? Email);
public sealed record RegistrationConfirmation(int RegistrationId, string EventTitle, string Message);
public sealed record Attendee(int RegistrationId, string FullName, string Email, DateTimeOffset RegisteredAt);
public sealed class RegistrationException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
