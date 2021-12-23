using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

namespace Pact.Impersonation;

/// <summary>
/// Windows-specific impersonation implementation
/// </summary>
public class WindowsImpersonator : IImpersonator
{
    private readonly ILogger<WindowsImpersonator> _logger;

    public WindowsImpersonator(ILogger<WindowsImpersonator> logger)
    {
        _logger = logger;
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);
        
    /// <inheritdoc />
    public void Execute(ImpersonationSettings settings, Action action)
    {
        var safeAccessTokenHandle = Logon(settings);

        WindowsIdentity.RunImpersonated(safeAccessTokenHandle, action);

        _logger.LogDebug("After impersonation: " + WindowsIdentity.GetCurrent().Name);
    }

    /// <inheritdoc />
    public T Execute<T>(ImpersonationSettings settings, Func<T> func)
    {
        var safeAccessTokenHandle = Logon(settings);

        return WindowsIdentity.RunImpersonated(safeAccessTokenHandle, func);
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(ImpersonationSettings settings, Func<Task> func)
    {
        var safeAccessTokenHandle = Logon(settings);

        await WindowsIdentity.RunImpersonatedAsync(safeAccessTokenHandle, func);

        _logger.LogDebug("After impersonation: " + WindowsIdentity.GetCurrent().Name);
    }

    /// <inheritdoc />
    public async Task<T> ExecuteAsync<T>(ImpersonationSettings settings, Func<Task<T>> func)
    {
        var safeAccessTokenHandle = Logon(settings);

        return await WindowsIdentity.RunImpersonatedAsync(safeAccessTokenHandle, func);
    }

    private SafeAccessTokenHandle Logon(ImpersonationSettings settings)
    {
        _logger.LogTrace("Setting up for impersonation");

        // Get the user token for the specified user, domain, and password using the 
        // unmanaged LogonUser method. 
        // The local machine name can be used for the domain name to impersonate a user on this machine.
        const int LOGON32_PROVIDER_DEFAULT = 0;
        //This parameter causes LogonUser to create a primary token. 
        const int LOGON32_LOGON_INTERACTIVE = 2;

        // Call LogonUser to obtain a handle to an access token. 
        var returnValue = LogonUser(settings.User, settings.Domain, settings.Password,
            LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
            out var safeAccessTokenHandle);

        if (!returnValue)
        {
            var ret = Marshal.GetLastWin32Error();
            var exc = new Win32Exception(ret);
            _logger.LogError(exc, "LogonUser failed with error code: {Code}", ret);
            throw exc;
        }

        // Check the identity.
        _logger.LogDebug("Before impersonation: " + WindowsIdentity.GetCurrent().Name);
        return safeAccessTokenHandle;
    }
}