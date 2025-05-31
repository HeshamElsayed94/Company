using Shared.DTOs;

namespace Service.Contracts;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);

    EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);

    EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation);

    void DeleteEmployeeFromCompany(Guid companyId, Guid id, bool trackChanges);

    void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool empTrackChanges);

}