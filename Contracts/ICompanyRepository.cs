using Entities.Models;

namespace Contracts;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);

    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges);

    Task<bool> CompanyExistsAsync(Guid companyId);

    void CreateCompany(Company company);

    void CreateCompanyCollection(IEnumerable<Company> companies);

    void DeleteCompany(Company company);
}