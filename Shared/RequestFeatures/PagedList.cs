namespace Shared.RequestFeatures;

public class PagedList<T> : List<T>
{
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new()
        {
            TotalCount = count,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalPage = (int)Math.Ceiling(count / (double)pageSize)
        };

        AddRange(items);
    }

    public MetaData MetaData { get; set; }

}