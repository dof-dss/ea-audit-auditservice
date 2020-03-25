using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EA.Audit.Common.Infrastructure
{
    public class ValidatorActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                    var result = new ContentResult();
                    IEnumerable<ModelError> allErrors = filterContext.ModelState.Values.SelectMany(v => v.Errors);
                    string content = JsonConvert.SerializeObject(allErrors,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    result.Content = content;
                    result.ContentType = "application/json";

                    filterContext.HttpContext.Response.StatusCode = 422;
                    filterContext.Result = result;
                
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}