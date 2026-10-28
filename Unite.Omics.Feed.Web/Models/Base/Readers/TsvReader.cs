using Unite.Essentials.Tsv;

namespace Unite.Omics.Feed.Web.Models.Base.Readers;

public class TsvReader<TModel> : IReader<TModel> where TModel : class, new()
{
    public string Format => "tsv";


    public TModel[] Read(StreamReader reader)
    {
        return TsvReader.Read<TModel>(reader).ToArray();
    }
}
