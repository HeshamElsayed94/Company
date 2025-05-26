using Contracts;
using Service.Contracts;

namespace Service;

public sealed class CompanyService(IRepositoryManager repository, ILoggerManager loggerManager) : ICompanyService
{

}