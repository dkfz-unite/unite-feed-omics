namespace Unite.Genome.Feed.Web.Handlers.Annotation.Converters;

using DataModels = Unite.Genome.Feed.Data.Models;
using DataResources = Unite.Genome.Annotations.Services.Models;

public static class EffectsDataConverter
{
    public static DataModels.Dna.EffectsDataModel[] Convert(DataResources.Dna.EffectsDataModel[] effectsResources)
    {
        var effectModels = new List<DataModels.Dna.EffectsDataModel>();

        foreach (var effectResource in effectsResources)
        {
            var effectModel = new DataModels.Dna.EffectsDataModel { VariantId = effectResource.VariantId };

            effectModel.AffectedTranscripts = effectResource.AffectedTranscripts?.Select(affectedTranscriptResource =>
            {
                var affectedTranscriptModel = new DataModels.Dna.AffectedTranscriptModel();

                Map(affectedTranscriptResource, affectedTranscriptModel);

                affectedTranscriptModel.VariantId = effectModel.VariantId;

                affectedTranscriptModel.Gene = new DataModels.GeneModel();

                Map(affectedTranscriptResource.Gene, affectedTranscriptModel.Gene);

                affectedTranscriptModel.Transcript = new DataModels.TranscriptModel();

                Map(affectedTranscriptResource.Transcript, affectedTranscriptModel.Transcript);

                affectedTranscriptModel.Transcript.Gene = affectedTranscriptModel.Gene;

                if (affectedTranscriptResource.Protein != null)
                {
                    affectedTranscriptModel.Protein = new DataModels.ProteinModel();

                    Map(affectedTranscriptResource.Protein, affectedTranscriptModel.Protein);

                    affectedTranscriptModel.Protein.Transcript = affectedTranscriptModel.Transcript;
                }

                return affectedTranscriptModel;

            }).ToArray();

            effectModels.Add(effectModel);
        }

        return effectModels.ToArray();
    }


    private static void Map(DataResources.Dna.AffectedTranscriptModel resource, DataModels.Dna.AffectedTranscriptModel model)
    {
        model.CDSStart = resource.CDSStart;
        model.CDSEnd = resource.CDSEnd;
        model.ProteinStart = resource.ProteinStart;
        model.ProteinEnd = resource.ProteinEnd;
        model.CDNAStart = resource.CDNAStart;
        model.CDNAEnd = resource.CDNAEnd;
        model.ProteinChange = resource.AminoAcidChange;
        model.CodonChange = resource.CodonChange;
        model.OverlapBpNumber = resource.OverlapBpNumber;
        model.OverlapPercentage = resource.OverlapPercentage;
        model.Distance = resource.Distance;
        model.Consequences = resource.Effects;
    }

    private static void Map(DataResources.GeneModel resource, DataModels.GeneModel model)
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

    private static void Map(DataResources.TranscriptModel resource, DataModels.TranscriptModel model)
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

    private static void Map(DataResources.ProteinModel resource, DataModels.ProteinModel model)
    {
        model.Id = resource.Id;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Length = resource.Length;
        model.IsCanonical = resource.IsCanonical;
    }
}
