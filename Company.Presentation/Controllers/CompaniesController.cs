using Asp.Versioning;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;

namespace CompanyEmployees.Presentation.Controllers;


[ApiVersion("1")]
[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManger service)
    : ControllerBase
{

    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Append("Allow", "GET, OPTIONS, POST, PUT,PATCH, DELETE");

        return Ok();
    }

    [HttpGet("collection/({ids})")]
    public async Task<IActionResult> GetCompanyCollection([FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        => Ok(await service.CompanyService.GetByIdsAsync(ids, false));

    /// <summary>
    /// Gets the list of all companies
    /// </summary>
    /// <returns>The companies list</returns>
    [HttpGet(Name = "GetCompanies")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetCompanies()
        => Ok(await service.CompanyService.GetAllCompaniesAsync(false));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetCompany(Guid id)
        => Ok(await service.CompanyService.GetCompanyAsync(id, false));

    [HttpPost(Name = "CreateCompany")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
    {
        var createdCompany = service.CompanyService.CreateCompanyAsync(company);

        return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, createdCompany);
    }

    [HttpPost("collection")]
    [ServiceFilter<ValidationFilterAttribute>]
    public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var (companies, ids) = await service.CompanyService
            .CreateCompanyCollectionAsync(companyCollection);

        return CreatedAtAction(nameof(GetCompanyCollection), new { ids }, companies);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter<ValidationFilterAttribute>]
    public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto companyForUpdate)
    {
        service.CompanyService.UpdateCompanyAsync(id, companyForUpdate, true);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> pathDoc)
    {
        if (pathDoc is null)
            return BadRequest($"{nameof(pathDoc)} object sent from client is null.");

        var (companyToPatch, companyEntity) = await service.CompanyService
            .GetCompanyForPatchAsync(id, true);

        pathDoc.ApplyTo(companyToPatch, ModelState);

        TryValidateModel(companyToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await service.CompanyService.SaveChangesForPatchAsync(companyToPatch, companyEntity);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteCompany(Guid id)
    {
        service.CompanyService.DeleteCompanyAsync(id, false);

        return NoContent();
    }
}