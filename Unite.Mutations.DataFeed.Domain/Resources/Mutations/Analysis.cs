using System;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class Analysis
    {
        public string Name { get; set; }
        public AnalysisType? Type { get; set; }
        public DateTime Date { get; set; }

        public File File { get; set; }

        public void Sanitise()
        {
            File?.Sanitise();
        }
    }
}
