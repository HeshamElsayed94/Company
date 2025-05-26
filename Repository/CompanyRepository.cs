using Contracts;
using Entities.Models;

namespace Repository;

public class CompanyRepository(RepositoryContext context)
    : RepositoryBase<Company>(context), ICompanyRepository
{

}