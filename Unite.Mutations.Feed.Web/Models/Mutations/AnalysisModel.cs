using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Web.Services.Mutations
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