using Entities.Models;
using Entities.Responses;
using Shared.DTOs;

namespace Service.Contracts;

public interface ICompanyService
{
    Task<ApiBaseResponse> GetAllCompaniesAsync(bool trackChanges);

    Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync
        (IEnumerable<CompanyForCreationDto> companyCollection);

    Task<ApiBaseResponse> GetCompanyAsync(Guid companyId, bool trackChanges);

    Task<bool> CompanyExistsAsync(Guid companyId);

    Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);

    Task DeleteCompanyAsync(Guid companyId, bool trackChanges);

    Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges);

    Task<(CompanyForUpdateDto CompanyToPatch, Company companyEntity)> GetCompanyForPatchAsync(Guid companyId, bool trackChanges);

    Task SaveChangesForPatchAsync(CompanyForUpdateDto companyToPatch, Company companyEntity);

}