using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers;

[ApiVersion("2")]
[Route("api/companies")]
[ApiController]
public class CompaniesV2Controller(IServiceManger service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await service.CompanyService
        .GetAllCompaniesAsync(trackChanges: false);

        var companiesV2 = companies.Select(x => $"{x.Name} V2");
        return Ok(companiesV2);
    }
}