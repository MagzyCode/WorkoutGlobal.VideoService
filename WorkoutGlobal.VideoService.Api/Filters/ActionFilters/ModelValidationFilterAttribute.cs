using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using WorkoutGlobal.VideoService.Api.Models;

namespace WorkoutGlobal.VideoService.Api.Filters.ActionFilters
{
    /// <summary>
    /// Attribute for validate incoming model on API endpoint.
    /// </summary>
    public class ModelValidationFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Actions after action method execution.
        /// </summary>
        /// <param name="context">Executed context.</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// Actions before action method execution.
        /// </summary>
        /// <param name="context">Executed context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dtoParam = context.ActionArguments
                .SingleOrDefault(x => x.Value.ToString().Contains("Dto")).Value;

            if (dtoParam is null)
                context.Result = new BadRequestObjectResult(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Incoming DTO model is null.",
                    Details = "Incoming DTO model not contain any value."
                });
            else if (!context.ModelState.IsValid)
            {
                var errorMessage = new StringBuilder();

                foreach (var error in context.ModelState.Values)
                    error.Errors.Select(x => x.ErrorMessage).ToList().ForEach((message) =>
                    {
                        errorMessage.Append(message + " ");
                    });

                context.Result = new BadRequestObjectResult(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Dto model isn't valid.",
                    Details = errorMessage.ToString()
                });
            }
        }
    }
}
