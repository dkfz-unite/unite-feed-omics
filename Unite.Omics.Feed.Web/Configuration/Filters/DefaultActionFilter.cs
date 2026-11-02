using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Configuration.Filters;

public class DefaultActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Form submissions combine binding errors with file and content validation.
        if (context.ActionDescriptor.Parameters.Any(parameter => typeof(SubmissionForm).IsAssignableFrom(parameter.ParameterType)))
            return;

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
