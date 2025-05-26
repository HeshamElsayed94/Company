using Contracts;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager(
    IRepositoryManager repository,
    ILoggerManager loggerManager,
    Lazy<ICompanyService> companyService,
    Lazy<IEmployeeService> employeeService) : IServiceManger
{


    public ICompanyService CompanyService => companyService.Value;

    public IEmployeeService EmployeeService => employeeService.Value;
}