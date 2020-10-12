using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

namespace Pact.Impersonation
{
    /// <inheritdoc />
    /// <summary>
    /// Windows-specific impersonation implementation
    /// Jonny: Was going to try and make this so we can support async.
    /// https://github.com/dotnet/runtime/issues/24009
    /// This can't be done until the above ticket has been implement in .net 5.0 (or whatever its going to be called)
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

        public void ExecuteAction(ImpersonationSettings settings, Action action)
        {
            _logger.LogInformation("Setting up for impersonation");

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
                Console.WriteLine("LogonUser failed with error code : {0}", ret);
                throw new Win32Exception(ret);
            }

            // Check the identity.
            _logger.LogInformation("Before impersonation: " + WindowsIdentity.GetCurrent().Name);

            // Note: if you want to run as unimpersonated, pass
            //       'SafeAccessTokenHandle.InvalidHandle' instead of variable 'safeAccessTokenHandle'
            WindowsIdentity.RunImpersonated(safeAccessTokenHandle, action);

            // Check the identity again.
            _logger.LogInformation("After impersonation: " + WindowsIdentity.GetCurrent().Name);
        }
    }
}
