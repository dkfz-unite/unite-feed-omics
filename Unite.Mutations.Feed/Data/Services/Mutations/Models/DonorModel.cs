using Unite.Data.Entities.Donors;

namespace Unite.Mutations.Feed.Data.Services.Mutations.Models
{
    public class DonorModel
    {
        public string Id { get; set; }

        public Donor ToEntity()
        {
            var donor = new Donor()
            {
                Id = Id
            };

            return donor;
        }
    }
}
