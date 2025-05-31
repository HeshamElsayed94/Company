namespace Shared.DTOs;

public record EmployeeForUpdateDto
{
    public string Name { get; init; }
    public int Age { get; init; }
    public string Position { get; init; }
}