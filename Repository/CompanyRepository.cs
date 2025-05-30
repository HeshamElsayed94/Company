using Contracts;
using Entities.Models;

namespace Repository;

public class CompanyRepository(RepositoryContext context)
    : RepositoryBase<Company>(context), ICompanyRepository
{
    public bool CompanyExists(Guid companyId) => ExistsByCondition(c => c.Id.Equals(companyId));

    public void CreateCompany(Company company) => Create(company);

    public void CreateCompanyCollection(IEnumerable<Company> companies) => CreateCollection(companies);

    public void DeleteCompany(Company company) => Delete(company);

    public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        => [.. FindAll(trackChanges).OrderBy(c => c.Name)];

    public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        => [.. FindByCondition(x => ids.Contains(x.Id), trackChanges)];

    public Company? GetCompany(Guid companyId, bool trackChanges) => FindByCondition(c => c.Id
    .Equals(companyId), trackChanges).FirstOrDefault();
}