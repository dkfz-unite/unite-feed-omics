using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;

namespace Unite.Omics.Feed.Web.Configuration.Options;

public class EnsemblDataOptions : IEnsemblDataOptions
{
    public string Host => Environment.GetEnvironmentVariable("UNITE_ENSEMBL_DATA_HOST");
}
