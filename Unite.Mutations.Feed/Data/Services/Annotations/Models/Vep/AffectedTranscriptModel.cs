using System.Collections.Generic;
using Unite.Data.Entities.Mutations;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class AffectedTranscriptModel
    {
        public int? CDNAStart { get; set; }
        public int? CDNAEnd { get; set; }
        public int? CDSStart { get; set; }
        public int? CDSEnd { get; set; }
        public int? ProteinStart { get; set; }
        public int? ProteinEnd { get; set; }
        public string AminoAcidChange { get; set; }
        public string CodonChange { get; set; }

        public GeneModel Gene { get; set; }
        public TranscriptModel Transcript { get; set; }
        public IEnumerable<ConsequenceModel> Consequences { get; set; }

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
