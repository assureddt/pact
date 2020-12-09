using Microsoft.AspNetCore.Mvc.Filters;
using Pact.Logging;
using Serilog;

namespace Pact.Web.Filters
{
    public class LoggingPageFilter : IPageFilter
    {
        private readonly IDiagnosticContext _diagnosticContext;
        public LoggingPageFilter(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            var name = context.HandlerMethod?.Name ?? context.HandlerMethod?.MethodInfo.Name;
            if (name != null)
            {
                _diagnosticContext.Set("RazorPageHandler", name);
            }

            LoggingExtensions.EnrichFromFilterContext(_diagnosticContext, context);
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context) { }
    }
}
