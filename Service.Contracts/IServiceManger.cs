namespace Service.Contracts;

public interface IServiceManger
{
    ICompanyService CompanyService { get; }

    IEmployeeService EmployeeService { get; }
}