using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;

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

    [HttpPost]
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
    {
        if (company is null)
            return BadRequest($"{nameof(CompanyForCreationDto)} object is null");

        var createdCompany = service.CompanyService.CreateCompany(company);

        return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, createdCompany);
    }
}