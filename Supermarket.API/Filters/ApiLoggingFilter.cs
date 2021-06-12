using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;

        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var controllerName = context.ActionDescriptor.RouteValues["controller"];
                var actionName = context.ActionDescriptor.RouteValues["action"];
                _logger.LogInformation($"{DateTime.Now} - Executing - {controllerName} - {actionName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogDebug($"{DateTime.Now} - Action Finished");
        }
    }
}
