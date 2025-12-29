using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using server.Middleware;

namespace server.Filter
{
    public class ValidationFilter : IActionFilter
    {
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var error = context.ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();
                //var errorMessage = string.Join(" | ", errors);

                // Trả về lỗi 400 với thông điệp lỗi
                // context.Result = new BadRequestObjectResult(new
                // {
                //     StatusCode = 400,
                //     ErrorMessage = errorMessage
                // });
                context.Result = new BadRequestObjectResult(new
                {
                    ErrorMessage = error
                });

                return;
            }
    }

        
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // throw new NotImplementedException();
        }
    }
}
