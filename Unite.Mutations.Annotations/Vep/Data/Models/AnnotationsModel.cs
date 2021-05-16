using System.Collections.Generic;

namespace Unite.Mutations.Annotations.Vep.Data.Models
{
    internal class AnnotationsModel
    {
        public MutationModel Mutation { get; set; }

        public IEnumerable<AffectedTranscriptModel> AffectedTranscripts { get; set; }
    }
}
