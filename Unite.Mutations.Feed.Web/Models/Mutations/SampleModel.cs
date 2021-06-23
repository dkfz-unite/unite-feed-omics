using System;
using Unite.Mutations.Feed.Data.Mutations.Models.Enums;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class SampleModel
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }

        public string DonorId { get; set; }
        public string SpecimenId { get; set; }
        public SpecimenType? SpecimenType { get; set; }

        public virtual void Sanitise()
        {
            Name = Name?.Trim();

            DonorId = DonorId?.Trim();
            SpecimenId = SpecimenId?.Trim();
        }
    }
}