using System;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Web.Resources.Mutations
{
    public class AnalysisResource
    {
        public string Name { get; set; }
        public AnalysisType? Type { get; set; }
        public DateTime Date { get; set; }

        public FileResource File { get; set; }

        public void Sanitise()
        {
            File?.Sanitise();
        }
    }
}