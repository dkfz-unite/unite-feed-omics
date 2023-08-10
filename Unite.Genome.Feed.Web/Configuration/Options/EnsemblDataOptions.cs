using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;

namespace Unite.Genome.Feed.Web.Configuration.Options;

public class EnsemblDataOptions : IEnsemblDataOptions
{
    public string Host => Environment.GetEnvironmentVariable("UNITE_ENSEMBL_DATA_HOST");
}
