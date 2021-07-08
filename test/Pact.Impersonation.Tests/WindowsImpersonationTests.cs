using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Pact.Impersonation.Tests
{
    public class WindowsImpersonationTests
    {
        private readonly ImpersonationSettings _settings;

        public WindowsImpersonationTests()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<WindowsImpersonationTests>()
                .Build();

            _settings = new ImpersonationSettings
            {
                User = config["Username"],
                Password = config["Password"],
                Domain = config["Domain"]
            };
        }

#if _WINDOWS
        [SkippableFact]
        public void SynchronousAction()
        {
            Skip.IfNot(OperatingSystem.IsWindows(), "Windows Only");
            if (!OperatingSystem.IsWindows()) return;
            Skip.If(string.IsNullOrWhiteSpace(_settings.User), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Password), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Domain), "No credentials in User Secrets");

            // arrange
            var loggerMock = new Mock<ILogger<WindowsImpersonator>>();
            var loggerObj = loggerMock.Object;
            var imp = new WindowsImpersonator(loggerObj);

            var original = WindowsIdentity.GetCurrent().Name;

            // act
            imp.Execute(_settings, () =>
            {
                if (!OperatingSystem.IsWindows()) return;
                loggerObj.LogInformation("Testing as: {Identity}", WindowsIdentity.GetCurrent().Name);
            });

            // assert
            loggerMock.Verify(m => m.Log(LogLevel.Information, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"Testing as: {_settings.Domain}\\{_settings.User}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Debug, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"Before impersonation: {original}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Debug, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"After impersonation: {original}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Trace, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == "Setting up for impersonation"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.VerifyNoOtherCalls();
        }

        [SkippableFact]
        public void SynchronousFunc()
        {
            Skip.IfNot(OperatingSystem.IsWindows(), "Windows Only");
            if (!OperatingSystem.IsWindows()) return;
            Skip.If(string.IsNullOrWhiteSpace(_settings.User), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Password), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Domain), "No credentials in User Secrets");

            // arrange
            var loggerMock = new Mock<ILogger<WindowsImpersonator>>();
            var loggerObj = loggerMock.Object;
            var imp = new WindowsImpersonator(loggerObj);

            var original = WindowsIdentity.GetCurrent().Name;

            // act
            var result = imp.Execute(_settings, () => !OperatingSystem.IsWindows() ? null : WindowsIdentity.GetCurrent().Name);

            // assert
            result.ShouldBe($"{_settings.Domain}\\{_settings.User}");

            loggerMock.Verify(m => m.Log(LogLevel.Debug, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"Before impersonation: {original}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Trace, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == "Setting up for impersonation"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.VerifyNoOtherCalls();
        }

        [SkippableFact]
        public async Task AsynchronousAction()
        {
            Skip.IfNot(OperatingSystem.IsWindows(), "Windows Only");
            if (!OperatingSystem.IsWindows()) return;
            Skip.If(string.IsNullOrWhiteSpace(_settings.User), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Password), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Domain), "No credentials in User Secrets");

            // arrange
            var loggerMock = new Mock<ILogger<WindowsImpersonator>>();
            var loggerObj = loggerMock.Object;
            var imp = new WindowsImpersonator(loggerObj);

            var original = WindowsIdentity.GetCurrent().Name;

            // act
            await imp.ExecuteAsync(_settings, () =>
            {
                if (!OperatingSystem.IsWindows()) return Task.CompletedTask;

                loggerObj.LogInformation("Testing as: {Identity}", WindowsIdentity.GetCurrent().Name);
                return Task.CompletedTask;
            });

            // assert
            loggerMock.Verify(m => m.Log(LogLevel.Information, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"Testing as: {_settings.Domain}\\{_settings.User}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Debug, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"Before impersonation: {original}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Debug, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"After impersonation: {original}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Trace, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == "Setting up for impersonation"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.VerifyNoOtherCalls();
        }

        [SkippableFact]
        public async Task AsynchronousFunc()
        {
            Skip.IfNot(OperatingSystem.IsWindows(), "Windows Only");
            if (!OperatingSystem.IsWindows()) return;
            Skip.If(string.IsNullOrWhiteSpace(_settings.User), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Password), "No credentials in User Secrets");
            Skip.If(string.IsNullOrWhiteSpace(_settings.Domain), "No credentials in User Secrets");

            // arrange
            var loggerMock = new Mock<ILogger<WindowsImpersonator>>();
            var loggerObj = loggerMock.Object;
            var imp = new WindowsImpersonator(loggerObj);

            var original = WindowsIdentity.GetCurrent().Name;

            // act
            var result = await imp.ExecuteAsync(_settings, () => Task.FromResult(!OperatingSystem.IsWindows() ? null : WindowsIdentity.GetCurrent().Name));

            // assert
            result.ShouldBe($"{_settings.Domain}\\{_settings.User}");

            loggerMock.Verify(m => m.Log(LogLevel.Debug, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == $"Before impersonation: {original}"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.Verify(m => m.Log(LogLevel.Trace, 0,
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == "Setting up for impersonation"),
                null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            loggerMock.VerifyNoOtherCalls();
        }
#else
    [Fact(Skip = "We only test this on Windows")]
    public void OnlyWindowsTests()
    {
    }
#endif
    }
}
