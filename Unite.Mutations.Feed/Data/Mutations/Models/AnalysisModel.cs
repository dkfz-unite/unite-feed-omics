using System;
using System.Collections.Generic;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Data.Mutations.Models
{
    public class AnalysisModel
    {
        public AnalysisType? Type { get; set; }
        public DateTime? Date { get; set; }

        public FileModel File { get; set; }

        public IEnumerable<AnalysedSampleModel> AnalysedSamples { get; set; }
    }
}
