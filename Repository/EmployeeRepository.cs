using Contracts;
using Entities.Models;

namespace Repository;

public class EmployeeRepository(RepositoryContext context)
    : RepositoryBase<Employee>(context), IEmployeeRepository
{

}