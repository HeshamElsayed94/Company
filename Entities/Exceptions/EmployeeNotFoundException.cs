namespace Entities.Exceptions;

public sealed class EmployeeNotFoundException(Guid employeeId)
    : NotFoundException($"The employee with id: {employeeId} doesn't exist in the database.")
{

}