using System.Collections.Generic;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class AnnotationModel
    {
        public MutationModel Mutation;

        public IEnumerable<AffectedTranscriptModel> AffectedTranscripts;
    }
}
