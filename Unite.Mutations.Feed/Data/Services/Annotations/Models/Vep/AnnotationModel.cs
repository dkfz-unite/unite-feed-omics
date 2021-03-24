using System.Collections.Generic;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class AnnotationModel
    {
        public MutationModel Mutation { get; set; }

        public IEnumerable<AffectedTranscriptModel> AffectedTranscripts { get; set; }
    }
}
