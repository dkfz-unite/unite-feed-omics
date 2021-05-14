using System;
using Unite.Data.Entities.Specimens.Tissues.Enums;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class TissueModel
    {
        /// <summary>
        /// External Id of the tissue
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// External Id of the initial donor in the hierarchy of specimens
        /// </summary>
        public string DonorId { get; set; }

        /// <summary>
        /// Type of the tissue (Control, Tumour)
        /// </summary>
        public TissueType? Type { get; set; }

        /// <summary>
        /// Tumour type of the tissue (Primary, Metastasis, Recurrent)
        /// </summary>
        public TumourType? TumourType { get; set; }

        /// <summary>
        /// Date when the tissue was extracted
        /// </summary>
        public DateTime? ExtractionDate { get; set; }

        public void Sanitise()
        {
            Id = Id?.Trim();
            DonorId = DonorId?.Trim();
        }
    }
}
