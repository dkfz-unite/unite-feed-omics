using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Mutations.Enums;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class SampleRepository : Repository<Sample>
    {
        public SampleRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public Sample Find(string donorId, string name, SampleType? typeId, SampleSubtype? subtypeId)
        {
            var sample = Find(sample =>
                sample.DonorId == donorId &&
                sample.Name == name &&
                sample.TypeId == typeId &&
                sample.SubtypeId == subtypeId);

            return sample;
        }

        protected override void Map(in Sample source, ref Sample target)
        {
            target.DonorId = source.DonorId;
            target.Name = source.Name;
            target.TypeId = source.TypeId;
            target.SubtypeId = source.SubtypeId;
            target.Date = source.Date;
        }
    }
}
