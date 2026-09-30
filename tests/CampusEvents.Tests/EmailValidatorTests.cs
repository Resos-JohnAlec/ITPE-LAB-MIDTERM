using CampusEvents;
using Moq;
using Xunit;

namespace CampusEvents.Tests;

public sealed class EmailValidatorTests
{
    private static (EmailValidator Validator, Mock<IUniversityDomainPolicy> Policy) CreateValidator()
    {
        var policy = new Mock<IUniversityDomainPolicy>(MockBehavior.Strict);
        policy.Setup(value => value.GetAllowedDomain()).Returns("dlsud.edu.ph");
        return (new EmailValidator(policy.Object), policy);
    }

    [Theory]
    [InlineData("student@dlsud.edu.ph")]
    [InlineData("john.alec@dlsud.edu.ph")]
    [InlineData("STUDENT@DLSUD.EDU.PH")]
    [InlineData("  student@dlsud.edu.ph  ")]
    [InlineData("student+club@dlsud.edu.ph")]
    public void Valid_campus_email_is_accepted(string email)
    {
        // Arrange
        var (validator, policy) = CreateValidator();
        // Act
        var result = validator.IsValid(email);
        // Assert: external configuration is supplied only by a mock.
        Assert.True(result);
        policy.Verify(value => value.GetAllowedDomain(), Times.Once);
    }

    [Theory]
    [InlineData("student@gmail.com")]
    [InlineData("student@dlsud.edu.ph.evil.test")]
    [InlineData("student@sub.dlsud.edu.ph")]
    [InlineData("student@fakedlsud.edu.ph")]
    public void Other_domains_are_rejected(string email)
    {
        var (validator, _) = CreateValidator();
        var result = validator.IsValid(email);
        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("student")]
    [InlineData("@dlsud.edu.ph")]
    [InlineData("student@@dlsud.edu.ph")]
    [InlineData("student name@dlsud.edu.ph")]
    [InlineData(".student@dlsud.edu.ph")]
    [InlineData("student.@dlsud.edu.ph")]
    [InlineData("student..name@dlsud.edu.ph")]
    [InlineData("Student <student@dlsud.edu.ph>")]
    [InlineData("student@dlsud.edu.ph\nInjected: header")]
    [InlineData("student\0@dlsud.edu.ph")]
    public void Malformed_input_is_rejected_before_consulting_policy(string? email)
    {
        var (validator, policy) = CreateValidator();
        var result = validator.IsValid(email);
        Assert.False(result);
        policy.Verify(value => value.GetAllowedDomain(), Times.Never);
    }

    [Theory]
    [InlineData(64, true)]
    [InlineData(65, false)]
    [InlineData(255, false)]
    public void Local_part_length_boundary_is_enforced(int length, bool expected)
    {
        var (validator, _) = CreateValidator();
        var result = validator.IsValid(new string('a', length) + "@dlsud.edu.ph");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void A_changed_external_domain_policy_is_mocked()
    {
        var policy = new Mock<IUniversityDomainPolicy>();
        policy.Setup(value => value.GetAllowedDomain()).Returns("example.edu");
        var validator = new EmailValidator(policy.Object);
        var result = validator.IsValid("student@example.edu");
        Assert.True(result);
        policy.Verify(value => value.GetAllowedDomain(), Times.Once);
    }

    [Fact]
    public void Invalid_refactor_input_never_attempts_a_database_connection()
    {
        var (validator, _) = CreateValidator();
        var service = new RegistrationService("not a connection string", validator);
        Assert.Throws<ArgumentException>(() => service.GetUserRegistration("' OR 1=1--"));
    }
}
