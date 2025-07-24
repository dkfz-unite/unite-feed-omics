using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Models.Rna.Readers.Tsv;

public class Reader : IReader<ExpressionModel>
{
    public string Format => "tsv";

    public ExpressionModel[] Read(StreamReader reader)
    {
        try
        {
            var tsv = reader.ReadToEnd();

            return TsvReader
                .Read<ExpressionModel>(tsv)
                .ToArrayOrNull();
        }
        catch
        {
            return null;
        }
    }
}
