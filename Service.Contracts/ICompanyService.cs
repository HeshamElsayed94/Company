using Shared.DTOs;

namespace Service.Contracts;

public interface ICompanyService
{
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

    CompanyDto GetCompany(Guid companyId, bool trackChanges);

    bool CompanyExists(Guid companyId);

    CompanyDto CreateCompany(CompanyForCreationDto company);

}