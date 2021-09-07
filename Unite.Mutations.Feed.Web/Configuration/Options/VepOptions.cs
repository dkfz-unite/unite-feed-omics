using System;
using Unite.Mutations.Annotations.Clients.Vep.Configuration.Options;

namespace Unite.Mutations.Feed.Web.Configuration.Options
{
    public class VepOptions : IVepOptions
    {
        public string Host => Environment.GetEnvironmentVariable("UNITE_VEP_HOST");
    }
}
