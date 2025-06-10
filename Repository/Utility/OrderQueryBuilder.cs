using System.Reflection;

namespace Repository.Utility;

public static class OrderQueryBuilder
{
    public static string? CreateOrderQuery<T>(string orderByQueryString)
    {
        var orderParams = orderByQueryString.Split(',', StringSplitOptions.RemoveEmptyEntries
           | StringSplitOptions.TrimEntries);

        var orderParts = new List<string>();

        foreach (var param in orderParams)
        {
            var propertyNameFromQurey = param.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];

            var objectProperty = typeof(T).GetProperty(propertyNameFromQurey,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (objectProperty is not null)
            {
                var direction = param.EndsWith(" desc", StringComparison.CurrentCultureIgnoreCase)
                    ? "descending"
                    : "ascending";

                orderParts.Add($"{objectProperty.Name} {direction}");
            }
        }

        if (orderParts.Count == 0)
            return null;

        return string.Join(',', orderParts);
    }
}