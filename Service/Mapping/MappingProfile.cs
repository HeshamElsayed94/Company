using Entities.Models;
using Riok.Mapperly.Abstractions;
using Shared.DTOs;

namespace Service.Mapping;

[Mapper]
public partial class MappingProfile
{
    private string FullAddress(Company company) => string.Join(' ', company.Address, company.Country);

    #region Company

    [MapPropertyFromSource(nameof(CompanyDto.FullAddress), Use = nameof(FullAddress))]
    public partial CompanyDto ToCompanyDto(Company company);

    public partial IEnumerable<CompanyDto> ToCompanyDto(IEnumerable<Company> company);

    public partial Company ToCompanyEntity(CompanyForCreationDto companyDto);

    public partial IEnumerable<Company> ToCompanyEntity(IEnumerable<CompanyForCreationDto> companyDtos);

    #endregion Company

    #region Employee

    public partial EmployeeDto ToEmployeeDto(Employee employee);

    public partial IEnumerable<EmployeeDto> ToEmployeeDto(IEnumerable<Employee> company);

    public partial Employee ToEmployeeEntity(EmployeeForCreationDto employeeDto);

    #endregion Employee
}