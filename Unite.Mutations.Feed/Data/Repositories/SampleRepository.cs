using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class SampleRepository : Repository<Sample>
    {
        public SampleRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in Sample source, ref Sample target)
        {
            target.DonorId = source.Donor?.Id ?? source.DonorId;

            target.Name = source.Name;
            target.TypeId = source.TypeId;
            target.SubtypeId = source.SubtypeId;
            target.Date = source.Date;
        }
    }
}
