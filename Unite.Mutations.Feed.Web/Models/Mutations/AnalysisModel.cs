using System;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class AnalysisModel
    {
        /// <summary>
        /// Type of the analysis (WGS, WES)
        /// </summary>
        public AnalysisType? Type { get; set; }

        /// <summary>
        /// Date of the analysis
        /// </summary>
        public DateTime? Date { get; set; }


        public FileModel File { get; set; }


        public void Sanitise()
        {
            File?.Sanitise();
        }
    }
}