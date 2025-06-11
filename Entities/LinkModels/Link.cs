namespace Entities.LinkModels;

public class Link
{
    public Link()
    {

    }

    public Link(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }

    public string? Href { get; set; }

    public string? Rel { get; set; }

    public string? Method { get; set; }
}

public class LinkResourceBase
{
    public List<Link> Links { get; set; } = [];
}

public class LinkCollectionWrapper<T> : LinkResourceBase
{
    public LinkCollectionWrapper()
    {
    }

    public LinkCollectionWrapper(List<T> value) => Value = value;

    public List<T> Value { get; set; } = [];
}