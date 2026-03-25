using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Constants;

namespace Unite.Omics.Feed.Web.Configuration.Options;

public class GenomeOptions : IGenomeOptions
{
    private static readonly string[] _supportedBuilds = [Genomes.GRCh37, Genomes.GRCh38];

    public string Build
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_GENOME_BUILD");

            if (string.IsNullOrWhiteSpace(option))
                return Genomes.GRCh37;

            var value = option.Trim();

            foreach (var build in _supportedBuilds)
            {
                if (string.Equals(value, build, StringComparison.InvariantCultureIgnoreCase))
                    return build;
            }

            throw new ArgumentException($"Invalid genome build: {value}. Must be one of: {string.Join(", ", _supportedBuilds)}");
        }
    }

    public int Version
    {
        get
        {
            return Build switch
            {
                Genomes.GRCh37 => 37,
                Genomes.GRCh38 => 38,
                _ => throw new InvalidOperationException($"Unsupported GRCh version: {Build}")
            };
        }
    }
}
