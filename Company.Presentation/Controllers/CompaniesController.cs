using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace Company.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManger service)
    : ControllerBase
{
    [HttpGet]
    public IActionResult GetCompanies()
    {
        try
        {
            return Ok(service.CompanyService.GetAllCompanies(false));
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
}