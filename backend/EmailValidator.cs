using System.Text.RegularExpressions;

namespace CampusEvents;

public interface IUniversityDomainPolicy
{
    string GetAllowedDomain();
}

public sealed class UniversityDomainPolicy(string domain) : IUniversityDomainPolicy
{
    public string GetAllowedDomain() => domain;
}

public sealed partial class EmailValidator(IUniversityDomainPolicy policy)
{
    // Deliberately supports ordinary ASCII campus mailboxes, not all RFC 5322 syntax.
    public bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var value = email.Trim();
        if (value.Length > 254 || !CampusEmailPattern().IsMatch(value)) return false;
        var parts = value.Split('@');
        if (parts[0].Length > 64 || parts[0].StartsWith('.') || parts[0].EndsWith('.') || parts[0].Contains("..")) return false;
        return string.Equals(parts[1], policy.GetAllowedDomain(), StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex(@"\A[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+@[A-Za-z0-9.-]+\z", RegexOptions.CultureInvariant)]
    private static partial Regex CampusEmailPattern();
}
