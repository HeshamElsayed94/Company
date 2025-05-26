using Service.Contracts;

namespace Service;

public sealed class ServiceManager(
    Lazy<ICompanyService> companyService,
    Lazy<IEmployeeService> employeeService) : IServiceManger
{


    public ICompanyService CompanyService => companyService.Value;

    public IEmployeeService EmployeeService => employeeService.Value;
}