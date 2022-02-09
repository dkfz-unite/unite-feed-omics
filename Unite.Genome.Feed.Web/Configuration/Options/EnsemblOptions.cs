using System;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;

namespace Unite.Genome.Feed.Web.Configuration.Options
{
    public class EnsemblOptions : IEnsemblOptions
    {
        public string Host => Environment.GetEnvironmentVariable("UNITE_ENSEMBL_HOST");
    }
}
