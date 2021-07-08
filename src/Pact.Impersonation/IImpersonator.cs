using System;
using System.Threading.Tasks;

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
        void Execute(ImpersonationSettings settings, Action action);

        /// <summary>
        /// Executes the provided action within the impersonated context
        /// </summary>
        /// <param name="settings">Passed on a per-action basis as it's not improbable you may need to impersonate with different credentials</param>
        /// <param name="func">The function to execute</param>
        T Execute<T>(ImpersonationSettings settings, Func<T> func);

        /// <summary>
        /// Executes the provided action within the impersonated context
        /// </summary>
        /// <param name="settings">Passed on a per-action basis as it's not improbable you may need to impersonate with different credentials</param>
        /// <param name="func">The function to execute</param>
        Task ExecuteAsync(ImpersonationSettings settings, Func<Task> func);

        /// <summary>
        /// Executes the provided action within the impersonated context
        /// </summary>
        /// <param name="settings">Passed on a per-action basis as it's not improbable you may need to impersonate with different credentials</param>
        /// <param name="func">The function to execute</param>
        Task<T> ExecuteAsync<T>(ImpersonationSettings settings, Func<Task<T>> func);
    }
}
