using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

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
}