using System.Threading.Tasks;
using Unite.Genome.Annotations.Clients.Vep.Configuration.Options;
using Unite.Genome.Annotations.Clients.Vep.Resources;

namespace Unite.Genome.Annotations.Clients.Vep
{
    internal class VepApiClient
    {
        private const string _annotationUrl = @"/api/vep?input={0}";
        private const string _annotationsUrl = @"/api/vep";

        private readonly IVepOptions _options;

        public VepApiClient(IVepOptions options)
        {
            _options = options;
        }

        public async Task<MutationResource> LoadAnnotations(string vepCode)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationUrl, vepCode);

            var resource = await httpClient.GetAsync<MutationResource>(url);

            return resource;
        }

        public async Task<MutationResource[]> LoadAnnotations(string[] vepCodes)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationsUrl);

            var resources = await httpClient.PostAsync<MutationResource[], string[]>(url, vepCodes);

            return resources;
        }
    }
}
