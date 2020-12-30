using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class GeneRepository : Repository<Gene>
    {
        public GeneRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public Gene Find(string name)
        {
            var gene = Find(gene =>
                gene.Name == name);

            return gene;
        }

        protected override void Map(in Gene source, ref Gene target)
        {
            target.Name = source.Name;
        }
    }
}
