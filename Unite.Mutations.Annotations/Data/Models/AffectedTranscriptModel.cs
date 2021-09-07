using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Annotations.Data.Models
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

        public ConsequenceType[] Consequences { get; set; }

        public MutationModel Mutation { get; set; }
        public TranscriptModel Transcript { get; set; }
    }
}
