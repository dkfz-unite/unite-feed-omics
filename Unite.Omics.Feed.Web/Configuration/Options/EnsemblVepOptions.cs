using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;

namespace Unite.Omics.Feed.Web.Configuration.Options;

public class EnsemblVepOptions : IEnsemblVepOptions
{
    public string Host => Environment.GetEnvironmentVariable("UNITE_ENSEMBL_VEP_HOST");
}
