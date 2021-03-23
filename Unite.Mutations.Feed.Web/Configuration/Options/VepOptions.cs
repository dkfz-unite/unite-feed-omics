using Unite.Mutations.Feed.Annotations.VEP.Configuration.Options;

namespace Unite.Mutations.Feed.Web.Configuration.Options
{
    public class VepOptions : IVepOptions
    {
        public string Host => EnvironmentConfig.VepHost;
    }
}
