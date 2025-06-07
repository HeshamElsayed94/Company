namespace Contracts;

public interface IRepositoryManager
{
    ICompanyRepository Companies { get; }

    IEmployeeRepository Employees { get; }

    Task SaveAsync();
}