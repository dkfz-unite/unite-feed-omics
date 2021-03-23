using System.Collections.Generic;
using Unite.Data.Entities.Mutations;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class AffectedTranscriptModel
    {
        public int? CDNAStart;
        public int? CDNAEnd;
        public int? CDSStart;
        public int? CDSEnd;
        public int? ProteinStart;
        public int? ProteinEnd;
        public string AminoAcidChange;
        public string CodonChange;

        public GeneModel Gene;
        public TranscriptModel Transcript;
        public IEnumerable<ConsequenceModel> Consequences;

        public AffectedTranscript ToEntity()
        {
            var affectedTranscript = new AffectedTranscript
            {
                CDNAStart = CDNAStart,
                CDNAEnd = CDNAEnd,
                CDSStart = CDSStart,
                CDSEnd = CDSEnd,
                ProteinStart = ProteinStart,
                ProteinEnd = ProteinEnd,
                AminoAcidChange = AminoAcidChange,
                CodonChange = CodonChange
            };

            return affectedTranscript;
        }
    }
}
