using Unite.Genome.Annotations.Clients.Ensembl.Resources;
using Unite.Genome.Annotations.Clients.Ensembl.Resources.Vep;
using Unite.Genome.Annotations.Data.Models;
using Unite.Genome.Annotations.Data.Models.Variants;

namespace Unite.Genome.Annotations.Services.Vep;


internal class AnnotationsDataConverter
{
    /// <summary>
    /// Converts annotated variant resources to consequences data models.
    /// </summary>
    /// <param name="variants">Annotated variants to convert</param>
    /// <param name="genes">Annotated genes cache</param>
    /// <param name="transcripts">Annotated transcripts cache</param>
    /// <returns>List of variant consequences data models.</returns>
    public ConsequencesDataModel[] Convert(
        AnnotatedVariantResource[] variants,
        GeneResource[] genes,
        TranscriptResource[] transcripts
        )
    {
        var consequencesDataModels = variants.Select(variant =>
        {
            var consequencesDataModel = new ConsequencesDataModel();

            consequencesDataModel.Variant = new VariantModel();

            Map(variant, consequencesDataModel.Variant);

            if (variant.AffectedTranscripts != null)
            {
                consequencesDataModel.AffectedTranscripts = variant.AffectedTranscripts.Select(affectedTranscript =>
                {
                    var affectedTranscriptModel = new AffectedTranscriptModel();

                    Map(affectedTranscript, affectedTranscriptModel);

                    affectedTranscriptModel.Variant = consequencesDataModel.Variant;

                    if (!string.IsNullOrWhiteSpace(affectedTranscript.TranscriptId))
                    {
                        var transcript = transcripts.First(transcript => transcript.Id == affectedTranscript.TranscriptId);

                        affectedTranscriptModel.Transcript = new TranscriptModel();

                        Map(transcript, affectedTranscriptModel.Transcript);

                        if (!string.IsNullOrWhiteSpace(affectedTranscript.GeneId))
                        {
                            var gene = genes.First(gene => gene.Id == affectedTranscript.GeneId);

                            affectedTranscriptModel.Transcript.Gene = new GeneModel();

                            Map(gene, affectedTranscriptModel.Transcript.Gene);
                        }
                    }

                    return affectedTranscriptModel;

                }).ToArray();
            }

            return consequencesDataModel;

        }).ToArray();

        return consequencesDataModels;
    }


    private static void Map(AnnotatedVariantResource resource, VariantModel model)
    {
        model.Id = long.Parse(resource.Id);
    }

    private static void Map(AffectedTranscriptResource resource, AffectedTranscriptModel model)
    {
        model.CDSStart = resource.CDSStart;
        model.CDSEnd = resource.CDSEnd;
        model.ProteinStart = resource.ProteinStart;
        model.ProteinEnd = resource.ProteinEnd;
        model.CDNAStart = resource.CDNAStart;
        model.CDNAEnd = resource.CDNAEnd;
        model.AminoAcidChange = resource.AminoAcidChange;
        model.CodonChange = resource.CodonChange;
        model.Consequences = resource.Consequences;
        model.OverlapBpNumber = resource.OverlapBpNumber;
        model.OverlapPercentage = resource.OverlapPercentage;
        model.Distance = resource.Distance;
        model.Consequences = resource.Consequences;
    }

    private static void Map(GeneResource resource, GeneModel model)
    {
        model.EnsemblId = resource.Id;
        model.Symbol = resource.Symbol;
        model.Biotype = resource.Biotype;
        model.Chromosome = resource.Chromosome;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Strand = resource.Strand == 1;
    }

    private static void Map(TranscriptResource resource, TranscriptModel model)
    {
        model.EnsemblId = resource.Id;
        model.Symbol = resource.Symbol;
        model.Biotype = resource.Biotype;
        model.Chromosome = resource.Chromosome;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Strand = resource.Strand == 1;

        if (resource.Protein != null)
        {
            model.Protein = new ProteinModel();

            Map(resource.Protein, model.Protein);
        }
    }

    private static void Map(ProteinResource resource, ProteinModel model)
    {
        model.EnsemblId = resource.Id;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Length = resource.Length;
    }
}
