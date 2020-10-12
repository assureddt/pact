using System;

namespace Pact.Impersonation
{
    /// <summary>
    /// Interface onto execution of an action in an impersonated context (typically file-system)
    /// Reason for this being here is so we could have separate libraries providing the actual implementation (e.g. for Windows and Linux etc.)
    /// </summary>
    public interface IImpersonator
    {
        /// <summary>
        /// Executes the provided action within the impersonated context
        /// </summary>
        /// <param name="settings">Passed on a per-action basis as it's not improbable you may need to impersonate with different credentials</param>
        /// <param name="action">The action to execute</param>
        void ExecuteAction(ImpersonationSettings settings, Action action);
    }
}
