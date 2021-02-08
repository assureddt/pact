using System;
using Pact.Web.ErrorHandling.Interfaces;

namespace Pact.Web.ErrorHandling.Settings
{
    public class ErrorHandlerSettings
    {
        /// <summary>
        /// If you'd prefer the Json responses are returned as HTTP 200 for you to handle differently, enable this
        /// </summary>
        public bool AjaxErrorsAsSuccess { get; set; }

        /// <summary>
        /// Discouraged, but enabling this will pass the exception message through to the Detail property.
        /// Otherwise, only the message from a caught FriendlyException will be included.
        /// </summary>
        public bool AjaxIncludeExceptionMessage { get; set; }

        /// <summary>
        /// Determines the response body returned for Json responses
        /// </summary>
        /// <example>
        /// JsonResponseFormatter = model => new {result = "ERROR", message = model.Details, code = model.Code};
        /// </example>
        public Func<IErrorDetails, object> JsonResponseFormatter { get; set; } = model => new { message = model.Details };

        /// <summary>
        /// Convenience helper to get preconfigured settings appropriate for some pre-existing scenarios
        /// </summary>
        /// <remarks>Specifically: AjaxErrorsAsSuccess & IncludeExceptionMessage & "ERROR" as a result property on the response object</remarks>
        /// <param name="opts"></param>
        public static void LaxDefaults(ErrorHandlerSettings opts)
        {
            opts.AjaxErrorsAsSuccess = true;
            opts.AjaxIncludeExceptionMessage = true;
            opts.JsonResponseFormatter = model => new
            {
                result = "ERROR",
                message = model.Details
            };
        }
    }
}
