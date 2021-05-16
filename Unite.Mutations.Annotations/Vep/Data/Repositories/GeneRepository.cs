using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Vep.Data.Models;

namespace Unite.Mutations.Annotations.Vep.Data.Repositories
{
    internal class GeneRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly BiotypeRepository _biotypeRepository;


        public GeneRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _biotypeRepository = new BiotypeRepository(dbContext);
        }

        public Gene FindOrCreate(GeneModel geneModel)
        {
            return Find(geneModel.EnsemblId) ?? Create(geneModel);
        }

        public Gene Find(GeneModel geneModel)
        {
            return Find(geneModel.EnsemblId);
        }

        public Gene Create(GeneModel geneModel)
        {
            var gene = Convert(geneModel);

            if(!string.IsNullOrWhiteSpace(geneModel.Biotype))
            {
                gene.Biotype = _biotypeRepository.FindOrCreate(geneModel.Biotype);
            }

            _dbContext.Genes.Add(gene);
            _dbContext.SaveChanges();

            return gene;
        }

        public IEnumerable<Gene> CreateMissing(IEnumerable<GeneModel> geneModels)
        {
            var genesToAdd = new List<Gene>();

            foreach(var geneModel in geneModels)
            {
                var gene = Find(geneModel.EnsemblId);

                if(gene == null)
                {
                    gene = Convert(geneModel);

                    if (!string.IsNullOrWhiteSpace(geneModel.Biotype))
                    {
                        gene.Biotype = _biotypeRepository.FindOrCreate(geneModel.Biotype);
                    }

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


        private Gene Find(string ensemblId)
        {
            var gene = _dbContext.Genes.FirstOrDefault(gene =>
                gene.Info.EnsemblId == ensemblId
            );

            return gene;
        }

        private Gene Convert(GeneModel geneModel)
        {
            var gene = new Gene();

            gene.Symbol = geneModel.Symbol;
            gene.Strand = geneModel.Strand;

            gene.Info = new GeneInfo
            {
                EnsemblId = geneModel.EnsemblId
            };

            return gene;
        }
    }
}
