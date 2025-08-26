using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_P26.Filters
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        override public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            if(context.HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new JsonResult(new
                {
                    status = 401,
                    message = "UnAuthorized"
                });
            }
        }

        override public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
            base.OnActionExecuted(context);
        }
    }
}
