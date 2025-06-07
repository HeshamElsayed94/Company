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

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation)
    {
        var companyExist = await repository.Companies.CompanyExistsAsync(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.ToEmployeeEntity(employeeForCreation);

        repository.Employees.CreateEmployeeForCompany(companyId, employeeEntity);
        await repository.SaveAsync();

        return _mapper.ToEmployeeDto(employeeEntity);
    }

    public async Task DeleteEmployeeFromCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var companyExist = await repository.Companies.CompanyExistsAsync(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employeeForCompany = await repository.Employees.GetEmployeeAsync(companyId, id, trackChanges)
            ?? throw new EmployeeNotFoundException(id);

        repository.Employees.DeleteEmployee(employeeForCompany);
        await repository.SaveAsync();

    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var companyExist = await repository.Companies.CompanyExistsAsync(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employee = await repository.Employees.GetEmployeeAsync(companyId, id, trackChanges)
            ?? throw new EmployeeNotFoundException(id);

        return _mapper.ToEmployeeDto(employee);

    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
        (Guid companyId, Guid id, bool empTrackChanges)
    {
        var companyExist = await repository.Companies.CompanyExistsAsync(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = await repository.Employees.GetEmployeeAsync(companyId, id, empTrackChanges)
            ?? throw new EmployeeNotFoundException(id);

        var employeeToPatch = _mapper.ToEmployeeForUpdateDto(employeeEntity);

        return (employeeToPatch, employeeEntity);
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
    {
        var company = await repository.Companies.GetCompanyAsync(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        var employees = await repository.Employees.GetEmployeesAsync(companyId, trackChanges);

        return [.. _mapper.ToEmployeeDto(employees)];

    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.UpdateEmployee(employeeToPatch, employeeEntity);
        await repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool empTrackChanges)
    {
        var companyExists = await repository.Companies.CompanyExistsAsync(companyId);

        if (!companyExists)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = await repository.Employees.GetEmployeeAsync(companyId, id, empTrackChanges)
            ?? throw new EmployeeNotFoundException(id);

        _mapper.UpdateEmployee(employeeForUpdate, employeeEntity);

        await repository.SaveAsync();
    }
}