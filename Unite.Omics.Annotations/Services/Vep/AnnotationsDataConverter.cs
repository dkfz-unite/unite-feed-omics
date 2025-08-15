using Unite.Omics.Annotations.Clients.Ensembl.Resources;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Vep;
using Unite.Omics.Annotations.Services.Models;
using Unite.Omics.Annotations.Services.Models.Dna;

namespace Unite.Omics.Annotations.Services.Vep;


internal static class AnnotationsDataConverter
{
    /// <summary>
    /// Converts annotated variant resources to consequences data models.
    /// </summary>
    /// <param name="variantResources">Annotated variants to convert</param>
    /// <param name="geneResources">Annotated genes cache</param>
    /// <param name="transcriptResources">Annotated transcripts cache</param>
    /// <returns>List of variant consequences data models.</returns>
    public static EffectsDataModel[] Convert(AnnotatedVariantResource[] variantResources, GeneResource[] geneResources, TranscriptResource[] transcriptResources)
    {
        var effectsDataModels = new List<EffectsDataModel>();

        foreach (var variantResource in variantResources)
        {
            var effectsDataModel = new EffectsDataModel() { VariantId = variantResource.VariantId }; 

            effectsDataModel.AffectedTranscripts = variantResource.AffectedTranscripts?.Select(affectedTranscript =>
            {
                var affectedTranscriptModel = new AffectedTranscriptModel();

                Map(affectedTranscript, affectedTranscriptModel);

                affectedTranscriptModel.VariantId = variantResource.VariantId;

                if (!string.IsNullOrWhiteSpace(affectedTranscript.GeneId))
                {
                    var geneResource = geneResources.First(gene => gene.Id == affectedTranscript.GeneId);
                    
                    affectedTranscriptModel.Gene = new GeneModel();
                    Map(geneResource, affectedTranscriptModel.Gene);
                }

                if (!string.IsNullOrWhiteSpace(affectedTranscript.TranscriptId))
                {
                    var transcriptResource = transcriptResources.First(transcript => transcript.Id == affectedTranscript.TranscriptId);

                    affectedTranscriptModel.Transcript = new TranscriptModel();

                    Map(transcriptResource, affectedTranscriptModel.Transcript);

                    affectedTranscriptModel.Transcript.Gene = affectedTranscriptModel.Gene;


                    if (transcriptResource.Protein != null)
                    {
                        affectedTranscriptModel.Protein = new ProteinModel();

                        Map(transcriptResource.Protein, affectedTranscriptModel.Protein);

                        affectedTranscriptModel.Protein.Transcript = affectedTranscriptModel.Transcript;
                    }
                }

                return affectedTranscriptModel;

            }).ToArray();

            effectsDataModels.Add(effectsDataModel);
        }

        return effectsDataModels.ToArray();
    }


    private static void Map(AffectedTranscriptResource resource, AffectedTranscriptModel model)
    {
        model.CDSStart = resource.CDSStart;
        model.CDSEnd = resource.CDSEnd;
        model.ProteinStart = resource.ProteinStart;
        model.ProteinEnd = resource.ProteinEnd;
        model.CDNAStart = resource.CDNAStart;
        model.CDNAEnd = resource.CDNAEnd;
        model.AminoAcidChange = resource.ProteinChange;
        model.CodonChange = resource.CodonChange;
        model.Effects = resource.Effects;
        model.OverlapBpNumber = resource.OverlapBpNumber;
        model.OverlapPercentage = resource.OverlapPercentage;
        model.Distance = resource.Distance;
        model.Effects = resource.Effects;
    }

    private static void Map(GeneResource resource, GeneModel model)
    {
        model.Id = resource.Id;
        model.Symbol = resource.Symbol;
        model.Description = resource.Description;
        model.Biotype = resource.Biotype;
        model.Chromosome = resource.Chromosome;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Strand = resource.Strand;
        model.ExonicLength = resource.ExonicLength;
    }

    private static void Map(TranscriptResource resource, TranscriptModel model)
    {
        model.Id = resource.Id;
        model.Symbol = resource.Symbol;
        model.Description = resource.Description;
        model.Biotype = resource.Biotype;
        model.IsCanonical = resource.IsCanonical;
        model.Chromosome = resource.Chromosome;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Strand = resource.Strand;
        model.ExonicLength = resource.ExonicLength;
    }

    private static void Map(ProteinResource resource, ProteinModel model)
    {
        model.Id = resource.Id;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Length = resource.Length;
        model.IsCanonical = resource.IsCanonical;
    }
}
