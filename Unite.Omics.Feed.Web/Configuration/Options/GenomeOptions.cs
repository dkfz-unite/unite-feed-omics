namespace Unite.Omics.Feed.Web.Configuration.Options;

public static class GenomeOptions
{
    private const string _grch37 = "GRCh37";
    private const string _grch38 = "GRCh38";
    private static readonly string[] _supportedBuilds = [_grch37, _grch38];

    public static string Build
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_GENOME_BUILD");

            if (string.IsNullOrWhiteSpace(option))
                return _grch37;

            var value = option.Trim();

            foreach (var build in _supportedBuilds)
            {
                if (string.Equals(value, build, StringComparison.InvariantCultureIgnoreCase))
                    return build;
            }

            throw new ArgumentException($"Invalid genome build: {value}. Must be one of: {string.Join(", ", _supportedBuilds)}");
        }
    }

    public static int Version
    {
        get
        {
            return Build switch
            {
                _grch37 => 37,
                _grch38 => 38,
                _ => throw new InvalidOperationException($"Unsupported GRCh version: {Build}")
            };
        }
    }
}
