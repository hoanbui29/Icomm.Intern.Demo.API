using System;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Icomm.Infrastructure.Filters
{
    public class ValidatorActionFilter: IActionFilter
    {
		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!filterContext.ModelState.IsValid)
				throw new ApiProblemDetailsException(filterContext.ModelState, statusCode: StatusCodes.Status400BadRequest);
			//filterContext.Result = new Apiexx
			//filterContext.Result = new BadRequestObjectResult(new ApiError("Validator error", filterContext.ModelState.Select(error => new ValidationError(error.Key, error.Value.Errors.FirstOrDefault()?.ErrorMessage))));
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{

		}
	}
}

