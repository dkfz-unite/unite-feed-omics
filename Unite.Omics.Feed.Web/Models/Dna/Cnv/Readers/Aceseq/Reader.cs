using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Models.Dna.Cnv.Readers.Aceseq;

public class Reader : IReader<VariantModel>
{
    public string Format => "aceseq";

    public VariantModel[] Read(StreamReader reader)
    {
        try
        {
            // Skip '#' in header row
            var buffer = new char[1];
            reader.Read(buffer, 0, 1);

            var tsv = reader.ReadToEnd();

            return TsvReader
                .Read<Variant>(tsv)
                .Select(variant => variant.Convert())
                .Where(variant => variant != null)
                .ToArrayOrNull();
        }
        catch
        {
            return null;
        }
    }
}
