using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager loggerManager) : IEmployeeService
{
    private readonly MappingProfile _mapper = new();

    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation)
    {
        var companyExist = repository.Companies.CompanyExists(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.ToEmployeeEntity(employeeForCreation);

        repository.Employees.CreateEmployeeForCompany(companyId, employeeEntity);
        repository.Save();

        return _mapper.ToEmployeeDto(employeeEntity);
    }

    public void DeleteEmployeeFromCompany(Guid companyId, Guid id, bool trackChanges)
    {
        var companyExist = repository.Companies.CompanyExists(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employeeForCompany = repository.Employees.GetEmployee(companyId, id, trackChanges)
            ?? throw new EmployeeNotFoundException(id);

        repository.Employees.DeleteEmployee(employeeForCompany);
        repository.Save();

    }

    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        var companyExist = repository.Companies.CompanyExists(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employee = repository.Employees.GetEmployee(companyId, id, trackChanges)
            ?? throw new EmployeeNotFoundException(id);

        return _mapper.ToEmployeeDto(employee);

    }

    public (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) GetEmployeeForPatch
        (Guid companyId, Guid id, bool empTrackChanges)
    {
        var companyExist = repository.Companies.CompanyExists(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = repository.Employees.GetEmployee(companyId, id, empTrackChanges)
            ?? throw new EmployeeNotFoundException(id);

        var employeeToPatch = _mapper.ToEmployeeForUpdateDto(employeeEntity);

        return (employeeToPatch, employeeEntity);
    }

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        var company = repository.Companies.GetCompany(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        var employees = repository.Employees.GetEmployees(companyId, trackChanges);

        return [.. _mapper.ToEmployeeDto(employees)];

    }

    public void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.UpdateEmployee(employeeToPatch, employeeEntity);
        repository.Save();
    }

    public void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool empTrackChanges)
    {
        var companyExists = repository.Companies.CompanyExists(companyId);

        if (!companyExists)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = repository.Employees.GetEmployee(companyId, id, empTrackChanges)
            ?? throw new EmployeeNotFoundException(id);

        _mapper.UpdateEmployee(employeeForUpdate, employeeEntity);

        repository.Save();
    }
}