using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;
using Shared.RequestFeatures;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager loggerManager) : IEmployeeService
{
    private readonly MappingProfile _mapper = new();

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation)
    {
        await CheckIfCompanyExists(companyId);

        var employeeEntity = _mapper.ToEmployeeEntity(employeeForCreation);

        repository.Employees.CreateEmployeeForCompany(companyId, employeeEntity);
        await repository.SaveAsync();

        return _mapper.ToEmployeeDto(employeeEntity);
    }

    public async Task DeleteEmployeeFromCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId);

        var employeeForCompany = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

        repository.Employees.DeleteEmployee(employeeForCompany);
        await repository.SaveAsync();

    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

        return _mapper.ToEmployeeDto(employee);

    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
        (Guid companyId, Guid id, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId);

        var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

        var employeeToPatch = _mapper.ToEmployeeForUpdateDto(employeeEntity);

        return (employeeToPatch, employeeEntity);
    }

    public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId);

        var pagedEmployees = await repository.Employees
            .GetEmployeesAsync(companyId, employeeParameters, trackChanges);

        return ( _mapper.ToEmployeeDto(pagedEmployees), pagedEmployees.MetaData);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.UpdateEmployee(employeeToPatch, employeeEntity);
        await repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId);

        var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

        _mapper.UpdateEmployee(employeeForUpdate, employeeEntity);

        await repository.SaveAsync();
    }

    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
    {
        return await repository.Employees.GetEmployeeAsync(companyId, id, trackChanges)
            ?? throw new EmployeeNotFoundException(id);
    }

    private async Task CheckIfCompanyExists(Guid companyId)
    {
        var companyExist = await repository.Companies.CompanyExistsAsync(companyId);

        if (!companyExist)
            throw new CompanyNotFoundException(companyId);
    }
}