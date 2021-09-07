using Unite.Mutations.Feed.Data.Mutations.Models.Enums;

namespace Unite.Mutations.Feed.Web.Services.Mutations
{
    public class SampleModel
    {
        public string Id { get; set; }
        public string DonorId { get; set; }
        public string SpecimenId { get; set; }
        public SpecimenType? SpecimenType { get; set; }

        public virtual void Sanitise()
        {
            Id = Id?.Trim();
            DonorId = DonorId?.Trim();
            SpecimenId = SpecimenId?.Trim();
        }
    }
}