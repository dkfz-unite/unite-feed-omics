namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public abstract class SpecimenModel
    {
        public string Id { get; set; }
        public string DonorId { get; set; }

        public virtual void Sanitise()
        {
            Id = Id?.Trim();
            DonorId = DonorId?.Trim();
        }
    }
}
