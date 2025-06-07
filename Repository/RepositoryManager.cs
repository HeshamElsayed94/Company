using Contracts;

namespace Repository;

public sealed class RepositoryManager(
    RepositoryContext context,
    Lazy<ICompanyRepository> companyRepository,
    Lazy<IEmployeeRepository> employeeRepository)
    : IRepositoryManager
{
    public ICompanyRepository Companies => companyRepository.Value;

    public IEmployeeRepository Employees => employeeRepository.Value;

    public async Task SaveAsync() => await context.SaveChangesAsync();
}