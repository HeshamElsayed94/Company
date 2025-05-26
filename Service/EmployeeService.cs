using Contracts;
using Service.Contracts;

namespace Service;

public sealed class EmployeeService(IRepositoryManager repository, ILoggerManager loggerManager) : IEmployeeService
{

}