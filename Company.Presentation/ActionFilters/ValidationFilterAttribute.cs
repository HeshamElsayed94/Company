using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CompanyEmployees.Presentation.ActionFilters;

public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var action = actionDescriptor?.ActionName ?? "UnkonnAction";
        var controller = actionDescriptor?.ControllerName ?? "UnkownController";

        var param = context.ActionArguments
            .FirstOrDefault(x => x.Value!.GetType().FullName!.Contains("Dto", StringComparison.OrdinalIgnoreCase)).Value;

        if (param is null)
        {
            context.Result = new BadRequestObjectResult($"Object is null. Controller: {controller}, action: {action}");
            return;
        }

        if (!context.ModelState.IsValid)
            context.Result = new UnprocessableEntityObjectResult(context.ModelState);

    }
}