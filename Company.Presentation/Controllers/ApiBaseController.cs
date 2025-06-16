using Entities.ErrorModel;
using Entities.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Presentation.Controllers;


public class ApiBaseController : ControllerBase
{
    protected IActionResult ProcessError(ApiBaseResponse baseResponse)
         => baseResponse switch
         {
             ApiNotFoundResponse notFoud => NotFound(new ErrorDetails
             {
                 Message = notFoud.Message,
                 StatusCode = StatusCodes.Status404NotFound
             }),
             ApiBadRequestResponse badRequest => BadRequest(new ErrorDetails
             {
                 Message = badRequest.Message,
                 StatusCode = StatusCodes.Status400BadRequest
             }),
             _ => throw new NotImplementedException()
         };

}