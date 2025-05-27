using Contracts;
using Entities.Exceptions;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager loggerManager) : IEmployeeService
{
    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        bool companyExist = repository.Companies.CompanyExists(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employee = repository.Employees.GetEmployee(companyId, id, trackChanges)
            ?? throw new EmployeeNotFoundException(id);

        var employeeDto = new MappingProfile().ToEmployeeDto(employee);

        return employeeDto;
    }

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        var company = repository.Companies.GetCompany(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        var employees = repository.Employees.GetEmployees(companyId, trackChanges);

        var employeesDto = new MappingProfile().ToEmployeeDto(employees);

        return employeesDto;
    }
}