using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Vep;
using Unite.Omics.Annotations.Services.Models.Dna;

namespace Unite.Omics.Annotations.Services.Vep;


internal class AnnotationsDataLoader
{
    private readonly EnsemblApiClient1 _ensemblApiClient;
    private readonly EnsemblVepApiClient _ensemblVepApiClient;
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;


    public AnnotationsDataLoader(
        IEnsemblDataOptions ensemblOptions,
        IEnsemblVepOptions vepOptions,
        IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _ensemblApiClient = new EnsemblApiClient1(ensemblOptions);
        _ensemblVepApiClient = new EnsemblVepApiClient(vepOptions);
        _dbContextFactory = dbContextFactory;
    }


    public async Task<EffectsDataModel[]> LoadData(string[] vepCodes, int grch)
    {
        var variants = await AnnotateVariants(vepCodes, grch);
        var genes = await AnnotateGenes(variants, grch);
        var transcripts = await AnnotateTranscripts(variants, grch);

        return AnnotationsDataConverter.Convert(variants, genes, transcripts);
    }


    private async Task<AnnotatedVariantResource[]> AnnotateVariants(string[] vepCodes, int grch)
    {
        var annotations = await _ensemblVepApiClient.LoadAnnotations(vepCodes);

        return Filter(annotations)?.ToArray();
    }

    private async Task<GeneResource[]> AnnotateGenes(AnnotatedVariantResource[] variants, int grch)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var allIdentifiers = variants
            .Where(variant => variant.AffectedTranscripts != null)
            .SelectMany(variant => variant.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.GeneId)
            .Distinct()
            .ToArray();

        //return await _ensemblApiClient.Find<GeneResource>(allIdentifiers, expand: false);

        var existingIdentifiers = allIdentifiers
            .Where(id => dbContext.Set<Gene>().AsNoTracking().Any(entity => entity.StableId == id))
            .ToArray();

        var newIdentifiers = allIdentifiers
            .Except(existingIdentifiers)
            // .Where(id => !existingIdentifiers.Contains(id))
            .ToArray();
            
        var existingResources = existingIdentifiers
            .Select(id => new GeneResource { Id = id })
            .ToArray();

        var newResources = await _ensemblApiClient.Find<GeneResource>(newIdentifiers, length: true, grch: grch);

        return Enumerable.Union(existingResources, newResources).ToArray();
    }

    private async Task<TranscriptResource[]> AnnotateTranscripts(AnnotatedVariantResource[] variants, int grch)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var allIdentifiers = variants
            .Where(variant => variant.AffectedTranscripts != null)
            .SelectMany(variant => variant.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.TranscriptId)
            .Distinct()
            .ToArray();

        //return await _ensemblApiClient.Find<TranscriptResource>(allIdentifiers, expand: true);

        var existingIdentifiers = allIdentifiers
            .Where(id => dbContext.Set<Transcript>().AsNoTracking().Any(entity => entity.StableId == id))
            .ToArray();

        var newIdentifiers = allIdentifiers
            .Except(existingIdentifiers)
            // .Where(id => !existingIdentifiers.Contains(id))
            .ToArray();

        var existingResources = existingIdentifiers
            .Select(id => new TranscriptResource { Id = id })
            .ToArray();

        var newResources = await _ensemblApiClient.Find<TranscriptResource>(newIdentifiers, length: true, expand: true, grch: grch);

        return Enumerable.Union(existingResources, newResources).ToArray();
    }


    private static IEnumerable<AnnotatedVariantResource> Filter(IEnumerable<AnnotatedVariantResource> variants)
    {
        if (variants == null)
        {
            yield break;
        }

        var groups = variants.GroupBy(variant => variant.VariantId);

        foreach (var group in groups)
        {
            var id = group.First().VariantId.ToString();

            var input = string.Join(Environment.NewLine, group.Select(variant => variant.Input));

            var affectedTranscripts = group
                .Where(variant => variant.AffectedTranscripts != null)
                .SelectMany(variant => variant.AffectedTranscripts)
                .DistinctBy(affectedTranscript => affectedTranscript.TranscriptId)
                .ToArray();

            yield return group.First() with
            {
                Id = id,
                Input = input,
                AffectedTranscripts = Filter(affectedTranscripts).ToArray()
            };
        }
    }

    private static IEnumerable<AffectedTranscriptResource> Filter(IEnumerable<AffectedTranscriptResource> features)
    {
        if (features == null)
        {
            yield break;
        }

        var groups = features.GroupBy(feature => feature.GeneId);

        foreach (var group in groups)
        {
            var groupHasCanonicalFeature = group.Any(feature => feature.Canonical == 1);

            foreach (var feature in group)
            {
                if (feature.OverlapPercentage == 100 || feature.Distance > 0)
                {
                    if (groupHasCanonicalFeature)
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
                else
                {
                    yield return feature;
                }
            }
        }
    }
}
