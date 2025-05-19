using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Vep;

namespace Unite.Omics.Annotations.Clients.Ensembl;

internal class EnsemblVepApiClient
{
    private const string _annotationUrl = @"/api/vep?input={0}";
    private const string _annotationsUrl = @"/api/vep";

    private readonly IEnsemblVepOptions _options;

    public EnsemblVepApiClient(IEnsemblVepOptions options)
    {
        _options = options;
    }

    public async Task<AnnotatedVariantResource> LoadAnnotations(string vepCode)
    {
        using var httpClient = new JsonHttpClient(_options.Host);

        var url = string.Format(_annotationUrl, vepCode);

        var resource = await httpClient.GetAsync<AnnotatedVariantResource>(url);

        return resource;
    }

    public async Task<AnnotatedVariantResource[]> LoadAnnotations(string[] vepCodes)
    {
        using var httpClient = new JsonHttpClient(_options.Host);

        var url = string.Format(_annotationsUrl);

        var resources = await httpClient.PostAsync<AnnotatedVariantResource[], string[]>(url, vepCodes);

        return resources;
    }
}
