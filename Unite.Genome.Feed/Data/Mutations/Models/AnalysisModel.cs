using System.Collections.Generic;
using Unite.Data.Entities.Genome.Mutations.Enums;

namespace Unite.Genome.Feed.Data.Mutations.Models
{
    public class AnalysisModel
    {
        public AnalysisType? Type { get; set; }

        public FileModel File { get; set; }

        public IEnumerable<AnalysedSampleModel> AnalysedSamples { get; set; }
    }
}
