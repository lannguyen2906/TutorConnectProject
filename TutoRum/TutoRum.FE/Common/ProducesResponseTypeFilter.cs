using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TutoRum.FE.Common
{
    public class ProducesResponseTypeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
                {
                    objectResult.StatusCode = StatusCodes.Status201Created;
                }
            }
        }
    }
}
