using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;
using Shared.RequestFeatures;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId:guid}/employees")]
[ApiController]
public class EmployeesController(IServiceManger service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEmployees(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
    {
        var (employees, metaData) = await service.EmployeeService
                .GetEmployeesAsync(companyId, employeeParameters, false);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEmployee(Guid companyId, Guid id)
        => Ok(await service.EmployeeService.GetEmployeeAsync(companyId, id, false));


    [HttpPost]
    [ServiceFilter<ValidationFilterAttribute>]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        var employeeDto = await service.EmployeeService
            .CreateEmployeeForCompanyAsync(companyId, employee);

        return CreatedAtAction(nameof(GetEmployee)
            , new { companyId, id = employeeDto.Id }, employeeDto);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter<ValidationFilterAttribute>]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employeeForUpdate)
    {
        await service.EmployeeService
              .UpdateEmployeeForCompanyAsync(companyId, id, employeeForUpdate, true);

        return NoContent();
    }


    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id
        , [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest($"{nameof(patchDoc)} object sent from client is null.");

        var (employeeToPatch, employeeEntity) = await service.EmployeeService
            .GetEmployeeForPatchAsync(companyId, id, true);

        patchDoc.ApplyTo(employeeToPatch, ModelState);

        TryValidateModel(employeeToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await service.EmployeeService.SaveChangesForPatchAsync(employeeToPatch, employeeEntity);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid id)
    {
        await service.EmployeeService.DeleteEmployeeFromCompanyAsync(companyId, id, false);

        return NoContent();
    }
}