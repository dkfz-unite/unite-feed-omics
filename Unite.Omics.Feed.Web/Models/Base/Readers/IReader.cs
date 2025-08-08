namespace Unite.Omics.Feed.Web.Models.Base.Readers;

public interface IReader<T> where T : class, new()
{
    string Format { get; }
    T[] Read(StreamReader reader);
}
