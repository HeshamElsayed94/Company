using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId:guid}/employees")]
[ApiController]
public class EmployeesController(IServiceManger service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEmployees(Guid companyId)
        => Ok(await service.EmployeeService.GetEmployeesAsync(companyId, false));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEmployee(Guid companyId, Guid id)
        => Ok(await service.EmployeeService.GetEmployeeAsync(companyId, id, false));


    [HttpPost]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        if (employee is null)
            return BadRequest($"{nameof(employee)} object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var employeeDto = await service.EmployeeService
            .CreateEmployeeForCompanyAsync(companyId, employee);

        return CreatedAtAction(nameof(GetEmployee)
            , new { companyId, id = employeeDto.Id }, employeeDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employeeForUpdate)
    {
        if (employeeForUpdate is null)
            return BadRequest($"{nameof(EmployeeForUpdateDto)} object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

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