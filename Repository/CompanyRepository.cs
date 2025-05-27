using Contracts;
using Entities.Models;

namespace Repository;

public class CompanyRepository(RepositoryContext context)
    : RepositoryBase<Company>(context), ICompanyRepository
{
    public bool CompanyExists(Guid companyId) => ExistsByCondition(c => c.Id.Equals(companyId));

    public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        => [.. FindAll(trackChanges).OrderBy(c => c.Name)];

    public Company? GetCompany(Guid companyId, bool trackChanges) => FindByCondition(c => c.Id
    .Equals(companyId), trackChanges).FirstOrDefault();
}