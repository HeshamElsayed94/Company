using Service.Contracts;

namespace Service;

public sealed class ServiceManager(
    Lazy<ICompanyService> companyService,
    Lazy<IEmployeeService> employeeService,
    Lazy<IAuthenticationService> authenticationService) : IServiceManger
{


    public ICompanyService CompanyService => companyService.Value;

    public IEmployeeService EmployeeService => employeeService.Value;

    public IAuthenticationService AuthenticationService => authenticationService.Value;
}