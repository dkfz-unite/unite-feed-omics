using System.Collections.Generic;

namespace Unite.Mutations.Annotations.Vep.Data.Models
{
    internal class AffectedTranscriptModel
    {
        public int? CDNAStart { get; set; }
        public int? CDNAEnd { get; set; }
        public int? CDSStart { get; set; }
        public int? CDSEnd { get; set; }
        public int? ProteinStart { get; set; }
        public int? ProteinEnd { get; set; }
        public string AminoAcidChange { get; set; }
        public string CodonChange { get; set; }

        public MutationModel Mutation { get; set; }
        public GeneModel Gene { get; set; }
        public TranscriptModel Transcript { get; set; }
        public IEnumerable<ConsequenceModel> Consequences { get; set; }
    }
}
