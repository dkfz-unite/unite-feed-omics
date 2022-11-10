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

    private readonly AnnotationsDataConverter _converter;


    public AnnotationsDataLoader(IEnsemblOptions ensemblOptions, IEnsemblVepOptions vepOptions)
    {
        _ensemblApiClient = new EnsemblApiClient(ensemblOptions);
        _ensemblVepApiClient = new EnsemblVepApiClient(vepOptions);
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

        var variants = annotations
            .GroupBy(annotation => annotation.VariantId)
            .Select(group => new AnnotatedVariantResource()
            {
                Id = group.Key.ToString(),
                AffectedTranscripts = group
                    .Where(annotation => annotation.AffectedTranscripts != null)
                    .SelectMany(annotation => annotation.AffectedTranscripts)
                    .GroupBy(affectedFeature => affectedFeature.GeneId)
                    .SelectMany(allFeatures =>
                    {
                        var canonicalFeatures = allFeatures.Where(feature => feature.Canonical == 1);
                        return canonicalFeatures.Any() ? canonicalFeatures : allFeatures;
                    })
                    .ToArray()
            })
            .ToArray();

        return variants;
    }

    private async Task<GeneResource[]> AnnotateGenes(AnnotatedVariantResource[] variants)
    {
        var geneIds = variants
            .Where(variant => variant.AffectedTranscripts != null)
            .SelectMany(variant => variant.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.GeneId)
            .Distinct()
            .ToArray();

        var genes = await _ensemblApiClient.Lookup<GeneResource>(geneIds, expand: false);

        return genes;
    }

    private async Task<TranscriptResource[]> AnnotateTranscripts(AnnotatedVariantResource[] variants)
    {
        var transcriptIds = variants
            .Where(variant => variant.AffectedTranscripts != null)
            .SelectMany(variant => variant.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.TranscriptId)
            .Distinct()
            .ToArray();

        var transcripts = await _ensemblApiClient.Lookup<TranscriptResource>(transcriptIds, expand: true);

        return transcripts;
    }
}
