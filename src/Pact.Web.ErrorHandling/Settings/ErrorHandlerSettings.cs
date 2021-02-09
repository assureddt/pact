using System;
using Pact.Web.ErrorHandling.Interfaces;

namespace Pact.Web.ErrorHandling.Settings
{
    public class ErrorHandlerSettings
    {
        /// <summary>
        /// If you'd prefer the Json responses are returned as HTTP 200 for you to handle differently, enable this
        /// </summary>
        public bool AjaxErrorsAsSuccess { get; set; } = true;

        /// <summary>
        /// Determines the response body returned for Json responses
        /// </summary>
        /// <example>
        /// JsonResponseFormatter = model => new {result = "ERROR", message = model.Details, code = model.Code};
        /// </example>
        public Func<IErrorDetails, object> JsonResponseFormatter { get; set; } = model => new
        {
            result = "ERROR",
            message = !string.IsNullOrWhiteSpace(model.Details) ? model.Details : "An error occurred with your request",
            code = model.Code
        };
    }
}
