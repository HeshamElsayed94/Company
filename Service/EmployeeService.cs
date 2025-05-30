using Contracts;
using Entities.Exceptions;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager loggerManager) : IEmployeeService
{
    private readonly MappingProfile _mapper = new();

    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation)
    {
        bool companyExist = repository.Companies.CompanyExists(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.ToEmployeeEntity(employeeForCreation);

        repository.Employees.CreateEmployeeForCompany(companyId, employeeEntity);
        repository.Save();

        return _mapper.ToEmployeeDto(employeeEntity);
    }

    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        bool companyExist = repository.Companies.CompanyExists(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employee = repository.Employees.GetEmployee(companyId, id, trackChanges)
            ?? throw new EmployeeNotFoundException(id);

        return _mapper.ToEmployeeDto(employee);

    }

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        var company = repository.Companies.GetCompany(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        var employees = repository.Employees.GetEmployees(companyId, trackChanges);

        return [.. _mapper.ToEmployeeDto(employees)];

    }
}