using System.Linq;
using Unite.Data.Entities.Mutations.Enums;
using Unite.Mutations.Annotations.Vep.Client.Resources;
using Unite.Mutations.Annotations.Vep.Data.Models;

namespace Unite.Mutations.Annotations.Vep.Services.Converters
{
    internal class AnnotationsModelConverter
    {
        public AnnotationsModel Convert(AnnotationsResource annotationsResource)
        {
            if (annotationsResource == null)
            {
                return null;
            }

            var annotationsModel = new AnnotationsModel
            {
                Mutation = GetMutationModel(annotationsResource),

                AffectedTranscripts = annotationsResource.AffectedTranscripts?.Select(affectedTranscriptResource =>
                {
                    var affectedTranscriptModel = GetAffectedTranscriptModel(affectedTranscriptResource);

                    affectedTranscriptModel.Mutation = GetMutationModel(annotationsResource);
                    affectedTranscriptModel.Gene = GetGeneModel(affectedTranscriptResource);
                    affectedTranscriptModel.Transcript = GetTranscriptModel(affectedTranscriptResource);
                    affectedTranscriptModel.Consequences = GetConsequenceModels(affectedTranscriptResource.Consequences);

                    return affectedTranscriptModel;

                }).ToArray()
            };

            return annotationsModel;
        }


        private static AffectedTranscriptModel GetAffectedTranscriptModel(AffectedTranscriptResource resource)
        {
            var model = new AffectedTranscriptModel
            {
                CDSStart = resource.CDSStart,
                CDSEnd = resource.CDSEnd,
                ProteinStart = resource.ProteinStart,
                ProteinEnd = resource.ProteinEnd,
                CDNAStart = resource.CDNAStart,
                CDNAEnd = resource.CDNAEnd,
                AminoAcidChange = resource.AminoAcidChange,
                CodonChange = resource.CodonChange,
            };

            return model;
        }

        private static MutationModel GetMutationModel(AnnotationsResource resource)
        {
            var model = new MutationModel()
            {
                Code = resource.Code
            };

            return model;
        }

        private static GeneModel GetGeneModel(AffectedTranscriptResource resource)
        {
            var model = new GeneModel
            {
                Strand = resource.Strand > 0 ? true : false,
                Symbol = resource.GeneSymbol,
                EnsemblId = resource.GeneId
            };

            return model;
        }

        private static TranscriptModel GetTranscriptModel(AffectedTranscriptResource resource)
        {
            var model = new TranscriptModel
            {
                Strand = resource.Strand > 0 ? true : false,
                Biotype = resource.TranscriptBiotype,
                EnsemblId = resource.TranscriptId
            };

            return model;
        }

        private static ConsequenceModel[] GetConsequenceModels(ConsequenceType[] resources)
        {
            var models = resources.Select(consequenceType =>
            {
                var consequenceModel = GetConsequenceModel(consequenceType);

                return consequenceModel;

            }).ToArray();

            return models;
        }

        private static ConsequenceModel GetConsequenceModel(ConsequenceType consequenceType)
        {
            var model = new ConsequenceModel()
            {
                Type = consequenceType
            };

            return model;
        }
    }
}
