using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class MutationRepository : Repository<Mutation>
    {
        private readonly GeneRepository _geneRepository;
        private readonly ContigRepository _contigRepository;

        public MutationRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
            _geneRepository = new GeneRepository(database, logger);
            _contigRepository = new ContigRepository(database, logger);
        }

        public Mutation Find(string code)
        {
            var mutation = Find(mutation =>
                mutation.Code == code);

            return mutation;
        }

        protected override void Map(in Mutation source, ref Mutation target)
        {
            target.Name = source.Name;
            target.Code = source.Code;
            target.Gene = GetOrCreateGene(source.Gene?.Name);
            target.ChromosomeId = source.ChromosomeId;
            target.Contig = GetOrCreateContig(source.Contig?.Value);
            target.SequenceTypeId = source.SequenceTypeId;
            target.Position = source.Position;
            target.TypeId = source.TypeId;
            target.ReferenceBase = source.ReferenceBase;
            target.AlternateBase = source.AlternateBase;
        }

        protected override IQueryable<Mutation> Include(IQueryable<Mutation> query)
        {
            var includeQuery = query
                .Include(mutation => mutation.Gene)
                .Include(mutation => mutation.Contig);

            return includeQuery;
        }


        private Gene GetOrCreateGene(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var entity = _geneRepository.Find(name);

            if(entity == null)
            {
                var gene = new Gene();
                gene.Name = name;

                entity = _geneRepository.Add(gene);
            }

            return entity;
        }

        private Contig GetOrCreateContig(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var entity = _contigRepository.Find(value);

            if(entity == null)
            {
                var contig = new Contig();
                contig.Value = value;

                entity = _contigRepository.Add(contig);
            }

            return entity;
        }
    }
}
