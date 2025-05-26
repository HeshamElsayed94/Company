using Contracts;

namespace Repository;

public sealed class RepositoryManager(
    RepositoryContext context,
    Lazy<ICompanyRepository> companyRepository,
    Lazy<IEmployeeRepository> employeeRepository)
    : IRepositoryManager
{
    public ICompanyRepository CompanyRepository => companyRepository.Value;

    public IEmployeeRepository EmployeeRepository => employeeRepository.Value;

    public void Save() => context.SaveChanges();
}