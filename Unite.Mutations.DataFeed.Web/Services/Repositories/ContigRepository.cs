using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class ContigRepository : Repository<Contig>
    {
        public ContigRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }


        public Contig Find(string value)
        {
            var contig = Find(contig =>
                contig.Value == value);

            return contig;
        }

        protected override void Map(in Contig source, ref Contig target)
        {
            target.Value = source.Value;
        }
    }
}
