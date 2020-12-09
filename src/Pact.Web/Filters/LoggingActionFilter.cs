using Microsoft.AspNetCore.Mvc.Filters;
using Pact.Logging;
using Serilog;

namespace Pact.Web.Filters
{
    public class LoggingActionFilter : IActionFilter
    {
        private readonly IDiagnosticContext _diagnosticContext;
        public LoggingActionFilter(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            LoggingExtensions.EnrichFromFilterContext(_diagnosticContext, context);
        }

        // Required by the interface
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
