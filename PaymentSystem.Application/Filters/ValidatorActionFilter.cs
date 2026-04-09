using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentSystem.Application.Filters
{
    public class ValidatorActionFilter: IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorActionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        foreach (var error in validationResult.Errors)
                            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                        var controller = context.Controller as Controller;
                        if (controller != null)
                        {
                            context.Result = new ViewResult
                            {
                                ViewData = controller.ViewData,
                                TempData = controller.TempData
                            };
                            controller.ViewData.Model = argument;
                        }
                        else
                        {
                            context.Result = new BadRequestObjectResult(context.ModelState);
                        }
                        return;
                    }
                }
            }
            await next();
        }
    }
}
