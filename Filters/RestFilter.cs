using ASP_P26.Models.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace ASP_P26.Filters
{
    public class RestFilter(String? Name = null) : ActionFilterAttribute
    {
        private readonly String? _Name = Name;

        override public void OnActionExecuting(ActionExecutingContext context)
        {
            RestResponse restResponse = new();
            restResponse.Meta.Method = context.HttpContext.Request.Method;
            if (_Name != null)
            {
                restResponse.Meta.ResourceName = _Name;
            }
            var field = context.Controller.GetType()
                .GetField("restResponse", BindingFlags.NonPublic | BindingFlags.Instance);
            
            field?.SetValue(context.Controller, restResponse);            
        }

        override public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
            base.OnActionExecuted(context);
        }
    }

}
