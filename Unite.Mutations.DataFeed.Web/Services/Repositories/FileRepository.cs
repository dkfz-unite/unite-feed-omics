using Microsoft.Extensions.Logging;
using Unite.Data.Entities;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class FileRepository : Repository<File>
    {
        public FileRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in File source, ref File target)
        {
            target.Name = source.Name;
            target.Link = source.Link;
            target.Created = source.Created;
            target.Updated = source.Updated;
        }
    }
}
