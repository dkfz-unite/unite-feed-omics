using Unite.Genome.Annotations.Clients.Vep.Configuration.Options;

namespace Unite.Genome.Feed.Web.Configuration.Options;

public class VepOptions : IVepOptions
{
    public string Host => Environment.GetEnvironmentVariable("UNITE_VEP_HOST");
}
