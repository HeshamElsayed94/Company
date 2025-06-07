using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CompanyRepository(RepositoryContext context)
    : RepositoryBase<Company>(context), ICompanyRepository
{
    public async Task<bool> CompanyExistsAsync(Guid companyId)
        => await FindByCondition(c => c.Id.Equals(companyId), false).AnyAsync();

    public void CreateCompany(Company company) => Create(company);

    public void CreateCompanyCollection(IEnumerable<Company> companies) => CreateCollection(companies);

    public void DeleteCompany(Company company) => Delete(company);

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        => await FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync();

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        => await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();

    public async Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges)
        => await FindByCondition(c => c.Id.Equals(companyId), trackChanges).FirstOrDefaultAsync();
}