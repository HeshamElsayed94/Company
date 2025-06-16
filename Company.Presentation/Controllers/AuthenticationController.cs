using Asp.Versioning;
using CompanyEmployees.Presentation.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;

namespace CompanyEmployees.Presentation.Controllers;

[ApiVersion("1")]
[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IServiceManger service) : ControllerBase
{
    [HttpPost]
    [ServiceFilter<ValidationFilterAttribute>]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        var result = await service.AuthenticationService.RegisterUser(userForRegistration);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.TryAddModelError(error.Code, error.Description);

            return BadRequest(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthentication)
    {
        var user = await service.AuthenticationService.ValidateUser(userForAuthentication);

        if (user is null)
            return Unauthorized();

        var tokenDto = await service.AuthenticationService.CreateToken(user, true);

        return Ok(tokenDto);
    }
}