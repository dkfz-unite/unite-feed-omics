using System.Linq;
using Unite.Data.Entities.Mutations.Enums;
using Unite.Mutations.Feed.Annotations.VEP.Resources;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep;
using Unite.Mutations.Feed.Web.Resources.Mutations.Converters.Helpers;

namespace Unite.Mutations.Feed.Web.Resources.Annotations.Converters
{
    public class AnnotationResourceConverter
    {
        public static AnnotationModel From(AnnotationResource resource)
        {
            var model = new AnnotationModel
            {
                Mutation = GetMutationModel(resource),
                AffectedTranscripts = GetAffectedTranscriptModels(resource.AffectedTranscripts)
            };

            return model;
        }

        private static MutationModel GetMutationModel(AnnotationResource resource)
        {
            var model = new MutationModel();

            model.Chromosome = resource.Chromosome;
            model.SequenceType = SequenceType.LinearGenomicDNA;
            model.Start = resource.Start;
            model.End = resource.End;
            model.ReferenceBase = resource.AlleleChange.Split('/')[0];
            model.AlternateBase = resource.AlleleChange.Split('/')[1];

            model.Code = MutationCodeHelper.GetCode(
                model.Chromosome,
                model.SequenceType,
                model.Start,
                model.End,
                model.ReferenceBase,
                model.AlternateBase
            );

            model.Type = MutationTypeHelper.GetMutationType(
                model.ReferenceBase,
                model.AlternateBase
            );

            return model;
        }

        private static AffectedTranscriptModel[] GetAffectedTranscriptModels(AffectedTranscriptResource[] resources)
        {
            if(resources != null)
            {
                return resources
                    .Select(resource => GetAffectedTranscriptModel(resource))
                    .ToArray();
            }
            else
            {
                return null;
            }
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

                Gene = GetGeneModel(resource),
                Transcript = GetTranscriptModel(resource),
                Consequences = GetConsequenceModels(resource.Consequences)
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

        private static ConsequenceModel[] GetConsequenceModels(ConsequenceType[] consequences)
        {
            return consequences
                .Select(consequence => GetConsequenceModel(consequence))
                .ToArray();
        }

        private static ConsequenceModel GetConsequenceModel(ConsequenceType consequenceType)
        {
            return new ConsequenceModel()
            {
                Type = consequenceType
            };
        }
    }
}