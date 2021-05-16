using Unite.Mutations.Annotations.Vep.Client.Resources;
using Unite.Mutations.Annotations.Vep.Configuration.Options;

namespace Unite.Mutations.Annotations.Vep.Client
{
    internal class VepAnnotationApiClient
    {
        private const string _annotationUrl = @"/api/hgvs?input={0}";
        private const string _annotationsUrl = @"/api/hgvs";

        private readonly IVepOptions _options;

        public VepAnnotationApiClient(IVepOptions options)
        {
            _options = options;
        }

        public AnnotationsResource LoadAnnotations(string hgvsCode)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationUrl, hgvsCode);

            var resource = httpClient.GetAsync<AnnotationsResource>(url).Result;

            return resource;
        }

        public AnnotationsResource[] LoadAnnotations(string[] hgvsCodes)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationsUrl);

            var resources = httpClient.PostAsync<AnnotationsResource[], string[]>(url, hgvsCodes).Result;

            return resources;
        }
    }
}
