using System;
using Unite.Mutations.Annotations.Clients.Ensembl.Configuration.Options;

namespace Unite.Mutations.Feed.Web.Configuration.Options
{
    public class EnsemblOptions : IEnsemblOptions
    {
        public string Host => Environment.GetEnvironmentVariable("UNITE_ENSEMBL_HOST");
    }
}
