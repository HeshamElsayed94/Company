using Contracts;
using Entities.Models;

namespace Repository;

public class EmployeeRepository(RepositoryContext context)
    : RepositoryBase<Employee>(context), IEmployeeRepository
{
    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public Employee? GetEmployee(Guid companyId, Guid id, bool trackChanges)
        => FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
            .FirstOrDefault();

    public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
        => [.. FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)];
}