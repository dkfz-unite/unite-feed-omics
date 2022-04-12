using System;
using Unite.Data.Entities.Genome.Mutations.Enums;

namespace Unite.Genome.Feed.Web.Services.Mutations
{
    public class AnalysisModel
    {
        /// <summary>
        /// Type of the analysis (WGS, WES)
        /// </summary>
        public AnalysisType? Type { get; set; }

        /// <summary>
        /// Date when the analysis was performed
        /// </summary>
        public DateTime? Date { get; set; }



        public void Sanitise()
        {
        }
    }
}