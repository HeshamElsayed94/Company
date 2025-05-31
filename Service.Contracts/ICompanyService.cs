using Entities.Models;
using Shared.DTOs;

namespace Service.Contracts;

public interface ICompanyService
{
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

    IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

    (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection);

    CompanyDto GetCompany(Guid companyId, bool trackChanges);

    bool CompanyExists(Guid companyId);

    CompanyDto CreateCompany(CompanyForCreationDto company);

    void DeleteCompany(Guid companyId, bool trackChanges);

    void UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges);

    (CompanyForUpdateDto CompanyToPatch, Company companyEntity) GetCompanyForPatch(Guid companyId, bool trackChanges);

    void SaveChangesForPatch(CompanyForUpdateDto companyToPatch, Company companyEntity);

}