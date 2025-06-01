using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManger service)
    : ControllerBase
{

    [HttpGet("collection/({ids})")]
    public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        => Ok(service.CompanyService.GetByIds(ids, false));


    [HttpGet]
    public IActionResult GetCompanies() => Ok(service.CompanyService.GetAllCompanies(false));

    [HttpGet("{id:guid}")]
    public ActionResult GetCompany(Guid id) => Ok(service.CompanyService.GetCompany(id, false));

    [HttpPost]
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
    {
        if (company is null)
            return BadRequest($"{nameof(CompanyForCreationDto)} object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var createdCompany = service.CompanyService.CreateCompany(company);

        return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, createdCompany);
    }

    [HttpPost("collection")]
    public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var (companies, ids) = service.CompanyService
            .CreateCompanyCollection(companyCollection);

        return CreatedAtAction(nameof(GetCompanyCollection), new { ids }, companies);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto companyForUpdate)
    {
        if (companyForUpdate is null)
            return BadRequest($"{nameof(CompanyForUpdateDto)} object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        service.CompanyService.UpdateCompany(id, companyForUpdate, true);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public IActionResult PartiallyUpdateCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> pathDoc)
    {
        if (pathDoc is null)
            return BadRequest($"{nameof(pathDoc)} object sent from client is null.");

        var (companyToPatch, companyEntity) = service.CompanyService
            .GetCompanyForPatch(id, true);

        pathDoc.ApplyTo(companyToPatch, ModelState);

        TryValidateModel(companyToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        service.CompanyService.SaveChangesForPatch(companyToPatch, companyEntity);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteCompany(Guid id)
    {
        service.CompanyService.DeleteCompany(id, false);

        return NoContent();
    }
}