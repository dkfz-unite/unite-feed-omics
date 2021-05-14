namespace Unite.Mutations.Feed.Mutations.Data.Models
{
    public class SpecimenModel
    {
        public string ReferenceId { get; set; }

        public DonorModel Donor { get; set; }

        public TissueModel Tissue { get; set; }
    }
}
