using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Models.Dna.Cnv.Readers.Tsv;

public class Reader : IReader<VariantModel>
{
    public string Format => "tsv";

    public VariantModel[] Read(StreamReader reader)
    {
        try
        {
            var tsv = reader.ReadToEnd();

            return TsvReader
                .Read<VariantModel>(tsv)
                .ToArrayOrNull();
        }
        catch
        {
            return null;
        }
    }
}
