using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;

namespace Unite.Genome.Feed.Web.Configuration.Options;

public class EnsemblVepOptions : IEnsemblVepOptions
{
    public string Host => Environment.GetEnvironmentVariable("UNITE_VEP_HOST");
}
