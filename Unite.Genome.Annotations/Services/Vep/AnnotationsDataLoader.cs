using Unite.Data.Entities.Genome;
using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Resources;
using Unite.Genome.Annotations.Clients.Ensembl.Resources.Vep;
using Unite.Genome.Annotations.Data.Models.Variants;

namespace Unite.Genome.Annotations.Services.Vep;


internal class AnnotationsDataLoader
{
    private readonly EnsemblApiClient _ensemblApiClient;
    private readonly EnsemblVepApiClient _ensemblVepApiClient;
    private readonly DomainDbContext _dbContext;

    private readonly AnnotationsDataConverter _converter;


    public AnnotationsDataLoader(
        IEnsemblOptions ensemblOptions,
        IEnsemblVepOptions vepOptions,
        DomainDbContext dbContext)
    {
        _ensemblApiClient = new EnsemblApiClient(ensemblOptions);
        _ensemblVepApiClient = new EnsemblVepApiClient(vepOptions);
        _dbContext = dbContext;
        _converter = new AnnotationsDataConverter();
    }

    public async Task<ConsequencesDataModel[]> LoadData(string[] vepCodes)
    {
        var variants = await AnnotateVariants(vepCodes);
        var genes = await AnnotateGenes(variants);
        var transcripts = await AnnotateTranscripts(variants);

        return _converter.Convert(variants, genes, transcripts);
    }


    private async Task<AnnotatedVariantResource[]> AnnotateVariants(string[] vepCodes)
    {
        var annotations = await _ensemblVepApiClient.LoadAnnotations(vepCodes);

        return Filter(annotations).ToArray();
    }

    private async Task<GeneResource[]> AnnotateGenes(AnnotatedVariantResource[] variants)
    {
        var allIdentifiers = variants
            .Where(variant => variant.AffectedTranscripts != null)
            .SelectMany(variant => variant.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.GeneId)
            .Distinct()
            .ToArray();

        var existingIdentifiers = allIdentifiers
            .Where(ensemblId => _dbContext.Set<Gene>().Any(entity => entity.Info.EnsemblId == ensemblId))
            .ToArray();

        var newIdentifiers = allIdentifiers
            .Where(ensemblId => !existingIdentifiers.Contains(ensemblId))
            .ToArray();

        var existingResources = existingIdentifiers
            .Select(ensemblId => new GeneResource { Id = ensemblId })
            .ToArray();

        var newResources = await _ensemblApiClient.Lookup<GeneResource>(newIdentifiers, expand: false);

        return Enumerable.Union(existingResources, newResources).ToArray();
    }

    private async Task<TranscriptResource[]> AnnotateTranscripts(AnnotatedVariantResource[] variants)
    {
        var allIdentifiers = variants
            .Where(variant => variant.AffectedTranscripts != null)
            .SelectMany(variant => variant.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.TranscriptId)
            .Distinct()
            .ToArray();

        var existingIdentifiers = allIdentifiers
            .Where(ensemblId => _dbContext.Set<Transcript>().Any(entity => entity.Info.EnsemblId == ensemblId))
            .ToArray();

        var newIdentifiers = allIdentifiers
            .Where(ensemblId => !existingIdentifiers.Contains(ensemblId))
            .ToArray();

        var existingResources = existingIdentifiers
            .Select(ensemblId => new TranscriptResource { Id = ensemblId })
            .ToArray();

        var newResources = await _ensemblApiClient.Lookup<TranscriptResource>(newIdentifiers, expand: true);

        return Enumerable.Union(existingResources, newResources).ToArray();
    }


    private static IEnumerable<AnnotatedVariantResource> Filter(IEnumerable<AnnotatedVariantResource> variants)
    {
        foreach (var variant in variants)
        {
            yield return variant with
            {
                AffectedTranscripts = Filter(variant.AffectedTranscripts).ToArray()
            };
        }
    }

    private static IEnumerable<AffectedTranscriptResource> Filter(IEnumerable<AffectedTranscriptResource> features)
    {
        var collectionHasCanonicalFeature = features.Any(feature => feature.Canonical == 1);

        foreach (var feature in features)
        {
            if (feature.OverlapPercentage == null && feature.Distance == null)
            {
                // SSM - return feature
                yield return feature;
            }
            else if (feature.OverlapPercentage != null && feature.Distance == null)
            {
                // CNV or SV
                if (feature.OverlapPercentage != 100)
                {
                    // Overlapped partly - return feature
                    yield return feature;
                }
                else
                {
                    // Overlapped fully
                    if (feature.IsIntergenic)
                    {
                        // Intergenic - ignore
                        continue;
                    }
                    else if (collectionHasCanonicalFeature)
                    {
                        // Return only canonical feature
                        if (feature.IsCanonical)
                        {
                            yield return feature;
                        }
                    }
                    else
                    {
                        // Return all features
                        yield return feature;
                    }
                }
            }
            else if (feature.OverlapPercentage == null && feature.Distance != null)
            {
                if (collectionHasCanonicalFeature)
                {
                    // Return only canonical feature
                    if (feature.IsCanonical)
                    {
                        yield return feature;
                    }
                }
                else
                {
                    // Return all features
                    yield return feature;
                }
            }
        }
    }
}
