using Unite.Mutations.Feed.Annotations.Common.Http;
using Unite.Mutations.Feed.Annotations.VEP.Configuration.Options;
using Unite.Mutations.Feed.Annotations.VEP.Resources;

namespace Unite.Mutations.Feed.Annotations.VEP
{
    public class VepAnnotationApiClient : IAnnotationApiClient<AnnotationResource>
    {
        private const string _annotationUrl = @"/api/hgvs?input={0}";
        private const string _annotationsUrl = @"/api/hgvs";

        private readonly IVepOptions _options;
        
        public VepAnnotationApiClient(IVepOptions options)
        {
            _options = options;
        }

        public AnnotationResource GetAnnotations(string hgvsCode)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationUrl, hgvsCode);

            var resources = httpClient.GetAsync<AnnotationResource[]>(url).Result;

            return resources?[0];
        }

        public AnnotationResource[] GetAnnotations(string[] hgvsCodes)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_annotationsUrl);

            var resources = httpClient.PostAsync<AnnotationResource[], string[]>(url, hgvsCodes).Result;

            return resources;
        }
    }
}
