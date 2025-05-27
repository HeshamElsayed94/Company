using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManger service)
    : ControllerBase
{
    [HttpGet]
    public IActionResult GetCompanies() => Ok(service.CompanyService.GetAllCompanies(false));

    [HttpGet("{id:guid}")]
    public ActionResult GetCompany(Guid id) => Ok(service.CompanyService.GetCompany(id, false));
}
