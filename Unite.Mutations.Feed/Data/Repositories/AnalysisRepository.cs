using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class AnalysisRepository : Repository<Analysis>
    {
        public AnalysisRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in Analysis source, ref Analysis target)
        {
            target.DonorId = source.Donor?.Id ?? source.DonorId;

            target.Name = source.Name;
            target.TypeId = source.TypeId;
            target.Date = source.Date;
            target.File = source.File;

            if(source.File != null)
            {
                if(target.File == null)
                {
                    target.File = new File();
                }

                Map(source.File, target.File);
            }
        }

        private void Map(in File source, File target)
        {
            target.Name = source.Name;
            target.Link = source.Link;
            target.Created = source.Created;
            target.Updated = source.Updated;
        }
    }
}
