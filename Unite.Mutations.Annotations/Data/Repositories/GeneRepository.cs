using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Data.Models;

namespace Unite.Mutations.Annotations.Data.Repositories
{
    internal class GeneRepository
    {
        private readonly DomainDbContext _dbContext;
        private readonly BiotypeRepository _biotypeRepository;


        public GeneRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _biotypeRepository = new BiotypeRepository(dbContext);
        }


        public Gene FindOrCreate(GeneModel geneModel)
        {
            return Find(geneModel) ?? Create(geneModel);
        }

        public Gene Find(GeneModel geneModel)
        {
            var gene = _dbContext.Genes.FirstOrDefault(gene =>
                gene.Info.EnsemblId == geneModel.EnsemblId
            );

            return gene;
        }

        public Gene Create(GeneModel geneModel)
        {
            var gene = Convert(geneModel);

            _dbContext.Genes.Add(gene);
            _dbContext.SaveChanges();

            return gene;
        }

        public IEnumerable<Gene> CreateMissing(IEnumerable<GeneModel> geneModels)
        {
            var genesToAdd = new List<Gene>();

            foreach (var geneModel in geneModels)
            {
                var gene = Find(geneModel);

                if (gene == null)
                {
                    gene = Convert(geneModel);

                    genesToAdd.Add(gene);
                }
            }

            if (genesToAdd.Any())
            {
                _dbContext.Genes.AddRange(genesToAdd);
                _dbContext.SaveChanges();
            }

            return genesToAdd;
        }


        private Gene Convert(GeneModel geneModel)
        {
            var gene = new Gene
            {
                Symbol = geneModel.Symbol,
                ChromosomeId = geneModel.Chromosome,
                Start = geneModel.Start,
                End = geneModel.End,
                Strand = geneModel.Strand,

                Info = new GeneInfo
                {
                    EnsemblId = geneModel.EnsemblId
                }
            };

            if (geneModel.Biotype != null)
            {
                gene.BiotypeId = _biotypeRepository.FindOrCreate(geneModel.Biotype).Id;
            }

            return gene;
        }
    }
}
