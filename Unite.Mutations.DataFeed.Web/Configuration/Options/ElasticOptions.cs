using Unite.Indices.Services.Configuration.Options;

namespace Unite.Mutations.DataFeed.Web.Configuration.Options
{
    public class ElasticOptions: IElasticOptions
    {
        public string Host => EnvironmentConfig.ElasticHost;
        public string User => EnvironmentConfig.ElasticUser;
        public string Password => EnvironmentConfig.ElasticPassword;
    }
}
