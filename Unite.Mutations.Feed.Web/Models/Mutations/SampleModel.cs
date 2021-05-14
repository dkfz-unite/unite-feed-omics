using System;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class SampleModel
    {
        /// <summary>
        /// External sample id (string)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the sample
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Date when the sample was created
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Tissue specimen data
        /// </summary>
        public TissueModel Tissue { get; set; }


        public virtual void Sanitise()
        {
            Id = Id?.Trim();
            Name = Name?.Trim();

            Tissue?.Sanitise();
        }
    }
}