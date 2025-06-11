using System.Dynamic;

namespace Contracts;

public interface IDataShaper<T>
{
    IEnumerable<ExpandoObject> ShapeDate(IEnumerable<T> entities, string? fieldsString);

    ExpandoObject ShapeDate(T entity, string? fieldsString);
}