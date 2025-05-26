using Contracts;
using Service.Contracts;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager loggerManager) : ICompanyService
{

}