using EmailSender.Factories.Email;
using EmailSender.Factories.Email.Interfaces;
using EmailSender.IntegrationTests.HelpersForTests;
using EmailSender.Services;
using FilmForumModels.Models.Email;
using FilmForumModels.Models.Settings;
using FluentAssertions;
using MailKit.Security;
using System.Net.Sockets;

namespace EmailSender.IntegrationTests;

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_ForProperData_SendsEmail()
    {
        //Arrange
        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create("filmforumwebtests@outlook.com");
        EmailSenderDetails emailSenderDetails = EmailServiceTestsHelper.EmailSenderDetails;
        SmtpSettings smtpSettings = EmailServiceTestsHelper.SmtpSettings;
        EmailService emailService = new();

        //Act
        string result = await emailService.SendEmailAsync(emailMessage, emailSenderDetails, smtpSettings);

        //Assert
        result.Should().Contain("OK");
    }

    [Theory]
    [InlineData("", "kd5#1D922d")]
    [InlineData("filmforumwebtests@outlook.com", "")]
    [InlineData("filmforumwebtests@outlook.com", "invalid_password")]
    [InlineData("emailwhichdoesnotexist@email.com", "kd5#1D922d")]
    public async Task SendEmailAsync_ForInvalidSenderCredentials_ThrowsAuthenticationException(string email, string password)
    {
        //Arrange
        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create("filmforumwebtests@outlook.com");
        EmailSenderDetails emailSenderDetails = new()
        {
            SenderName = "FilmForumTests",
            Email = email,
            Password = password
        };
        SmtpSettings smtpSettings = EmailServiceTestsHelper.SmtpSettings;
        EmailService emailService = new();

        //Act
        Func<Task<string>> action = () => emailService.SendEmailAsync(emailMessage, emailSenderDetails, smtpSettings);

        //Assert
        await action.Should().ThrowAsync<AuthenticationException>();
    }

    [Theory]
    [InlineData("smtp-mail.outlook.com", 11111)]
    [InlineData("invalid_host@invalidhost.com", 587)]
    public async Task SendEmailAsync_ForInvalidSmtpSettings_ThrowsSocketException(string host, int port)
    {
        //Arrange
        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create("filmforumwebtests@outlook.com");
        EmailSenderDetails emailSenderDetails = EmailServiceTestsHelper.EmailSenderDetails;
        SmtpSettings smtpSettings = new()
        {
            Host = host,
            Port = port,
            SecureSocketOptions = SecureSocketOptions.StartTls,
        };
        EmailService emailService = new();

        //Act
        Func<Task<string>> action = () => emailService.SendEmailAsync(emailMessage, emailSenderDetails, smtpSettings);

        //Assert
        await action.Should().ThrowAsync<SocketException>();
    }

    [Fact]
    public async Task SendEmailAsync_ForNullMessage_SendsEmail()
    {
        //Arrange
        EmailSenderDetails emailSenderDetails = EmailServiceTestsHelper.EmailSenderDetails;
        SmtpSettings smtpSettings = EmailServiceTestsHelper.SmtpSettings;
        EmailService emailService = new();

        //Act
        Func<Task<string>> action = () => emailService.SendEmailAsync(null!, emailSenderDetails, smtpSettings);

        //Assert
        await action.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task SendEmailAsync_ForNullSender_ThrowsNullReferenceException()
    {
        //Arrange
        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create("filmforumwebtests@outlook.com");
        SmtpSettings smtpSettings = EmailServiceTestsHelper.SmtpSettings;
        EmailService emailService = new();

        //Act
        Func<Task<string>> action = () => emailService.SendEmailAsync(emailMessage, null!, smtpSettings);

        //Assert
        await action.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task SendEmailAsync_ForNullSmtpSettings_ThrowsNullReferenceException()
    {
        //Arrange
        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create("filmforumwebtests@outlook.com");
        EmailSenderDetails emailSenderDetails = EmailServiceTestsHelper.EmailSenderDetails;
        EmailService emailService = new();

        //Act
        Func<Task<string>> action = () => emailService.SendEmailAsync(emailMessage, emailSenderDetails, null!);

        //Assert
        await action.Should().ThrowAsync<NullReferenceException>();
    }
}