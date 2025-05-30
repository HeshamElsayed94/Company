using Entities.Models;

namespace Contracts;

public interface ICompanyRepository
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);

    IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

    Company? GetCompany(Guid companyId, bool trackChanges);

    bool CompanyExists(Guid companyId);

    void CreateCompany(Company company);

    void CreateCompanyCollection(IEnumerable<Company> companies);

    void DeleteCompany(Company company);
}