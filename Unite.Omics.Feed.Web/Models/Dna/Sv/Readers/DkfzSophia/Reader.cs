using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Models.Dna.Sv.Readers.DkfzSophia;

public class Reader : IReader<VariantModel>
{
    public string Format => "dkfz/sophia";

    public VariantModel[] Read(StreamReader reader)
    {
        try
        {
            // Skip '#' in header row
            var buffer = new char[1];
            reader.Read(buffer, 0, 1);

            var tsv = reader.ReadToEnd();

            return TsvReader
                .Read<Entry>(tsv)
                .Select(entry => entry.Convert())
                .Where(model => model != null)
                .ToArrayOrNull();
        }
        catch
        {
            return null;
        }
    }
}
