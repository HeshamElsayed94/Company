using System.Linq.Dynamic.Core;
using System.Reflection;
using Entities.Models;

namespace Repository.Extensions;

public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees, uint minAge, uint maxAge)
        => employees.Where(e => e.Age >= minAge && e.Age <= maxAge);

    public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;
        return employees
            .Where(e => e.Name!.Contains(searchTerm));
    }

    public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return employees.OrderBy(e => e.Name);

        var orderParams = orderByQueryString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var propertyInfos = typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance);


        var orderParts = new List<string>();

        foreach (var param in orderParams)
        {
            var propertyNameFromQurey = param.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];

            var objectProperty = propertyInfos.FirstOrDefault(pi
                => pi.Name.Equals(propertyNameFromQurey, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty is not null)
            {
                var direction = param.EndsWith(" desc") ? "descending" : "ascending";

                orderParts.Add($"{objectProperty.Name} {direction}");
            }
        }

        if (orderParts.Count == 0)
            return employees.OrderBy(e => e.Name);

        var orderQuery = string.Join(',', orderParts);

        return employees.OrderBy(orderQuery);

    }

}