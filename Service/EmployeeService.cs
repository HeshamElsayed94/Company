using Contracts;
using Service.Contracts;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager loggerManager) : IEmployeeService
{

}