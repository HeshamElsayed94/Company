namespace Shared.DTOs;

public record CompanyForUpdateDto
{
    public string Name { get; init; }
    public string Address { get; init; }
    public string Country { get; init; }
    public ICollection<EmployeeForCreationDto> Employees { get; init; }
}