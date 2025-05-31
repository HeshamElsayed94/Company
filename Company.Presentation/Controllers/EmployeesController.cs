using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId:guid}/employees")]
[ApiController]
public class EmployeesController(IServiceManger service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetEmployees(Guid companyId)
        => Ok(service.EmployeeService.GetEmployees(companyId, false));

    [HttpGet("{id:guid}")]
    public IActionResult GetEmployee(Guid companyId, Guid id)
        => Ok(service.EmployeeService.GetEmployee(companyId, id, false));


    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        if (employee is null)
            return BadRequest($"{nameof(employee)} object is null");

        var employeeDto = service.EmployeeService
            .CreateEmployeeForCompany(companyId, employee);

        return CreatedAtAction(nameof(GetEmployee)
            , new { companyId, id = employeeDto.Id }, employeeDto);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employeeForUpdate)
    {
        if (employeeForUpdate is null)
            return BadRequest($"{nameof(EmployeeForUpdateDto)} object is null");

        service.EmployeeService
            .UpdateEmployeeForCompany(companyId, id, employeeForUpdate, true);

        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployee(Guid companyId, Guid id)
    {
        service.EmployeeService.DeleteEmployeeFromCompany(companyId, id, false);

        return NoContent();
    }
}