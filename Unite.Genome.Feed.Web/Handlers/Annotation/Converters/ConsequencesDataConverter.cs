namespace Unite.Genome.Feed.Web.Handlers.Annotation.Converters;

using DataModels = Unite.Genome.Feed.Data.Models;
using DataResources = Unite.Genome.Annotations.Services.Models;

public static class ConsequencesDataConverter
{
    public static DataModels.Variants.ConsequencesDataModel[] Convert(DataResources.Variants.ConsequencesDataModel[] consequenceResources)
    {
        var consequenceModels = new List<DataModels.Variants.ConsequencesDataModel>();

        foreach (var consequenceResource in consequenceResources)
        {
            var consequenceModel = new DataModels.Variants.ConsequencesDataModel();

            consequenceModel.Variant = new DataModels.Variants.VariantModel();

            Map(consequenceResource.Variant, consequenceModel.Variant);

            consequenceModel.AffectedTranscripts = consequenceResource.AffectedTranscripts?.Select(affectedTranscriptResource =>
            {
                var affectedTranscriptModel = new DataModels.Variants.AffectedTranscriptModel();

                Map(affectedTranscriptResource, affectedTranscriptModel);

                affectedTranscriptModel.Variant = consequenceModel.Variant;

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

            consequenceModels.Add(consequenceModel);
        }

        return consequenceModels.ToArray();
    }


    private static void Map(DataResources.Variants.VariantModel resource, DataModels.Variants.VariantModel model)
    {
        model.Id = resource.Id;
    }

    private static void Map(DataResources.Variants.AffectedTranscriptModel resource, DataModels.Variants.AffectedTranscriptModel model)
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
