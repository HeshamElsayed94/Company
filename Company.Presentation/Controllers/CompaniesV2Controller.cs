using Asp.Versioning;
using CompanyEmployees.Presentation.ApiBaseResponseExtensions;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;

namespace CompanyEmployees.Presentation.Controllers;

[ApiVersion("2")]
[Route("api/companies")]
[ApiController]
public class CompaniesV2Controller(IServiceManger service) : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await service.CompanyService
        .GetAllCompaniesAsync(trackChanges: false);

        var companiesV2 = companies.GetResult<IEnumerable<CompanyDto>>()
            .Select(x => $"{x.Name} V2");
        return Ok(companiesV2);
    }
}