using Unite.Mutations.Annotations.Vep.Client.Resources;
using Unite.Mutations.Annotations.Vep.Configuration.Options;

namespace Unite.Mutations.Annotations.Vep.Client
{
    internal class VepAnnotationApiClient
    {
        private const string _annotationUrl = @"/api/vep?input={0}";
        private const string _annotationsUrl = @"/api/vep";

        private readonly IVepOptions _options;

        public VepAnnotationApiClient(IVepOptions options)
        {
            _options = options;
        }

        public AnnotationsResource LoadAnnotations(string vepCode)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationUrl, vepCode);

            var resource = httpClient.GetAsync<AnnotationsResource>(url).Result;

            return resource;
        }

        public AnnotationsResource[] LoadAnnotations(string[] vepCodes)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationsUrl);

            var resources = httpClient.PostAsync<AnnotationsResource[], string[]>(url, vepCodes).Result;

            return resources;
        }
    }
}
