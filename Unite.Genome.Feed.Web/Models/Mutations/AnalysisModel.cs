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
        /// File generated during the analysis
        /// </summary>
        public FileModel File { get; set; }


        public void Sanitise()
        {
            File?.Sanitise();
        }
    }
}