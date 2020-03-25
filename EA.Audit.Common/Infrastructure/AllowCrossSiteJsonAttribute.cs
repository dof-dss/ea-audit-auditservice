using Microsoft.AspNetCore.Mvc.Filters;

namespace EA.Audit.Common.Infrastructure
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            base.OnActionExecuting(filterContext);
        }
    }
}
