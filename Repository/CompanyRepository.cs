using Contracts;
using Entities.Models;

namespace Repository;

public class CompanyRepository(RepositoryContext context)
    : RepositoryBase<Company>(context), ICompanyRepository
{
    public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        => [.. FindAll(trackChanges).OrderBy(c => c.Name)];
}