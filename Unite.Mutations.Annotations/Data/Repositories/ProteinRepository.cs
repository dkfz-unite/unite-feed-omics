using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Data.Models;

namespace Unite.Mutations.Annotations.Data.Repositories
{
    internal class ProteinRepository
    {
        private readonly DomainDbContext _dbContext;


        public ProteinRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Protein FindOrCreate(ProteinModel proteinModel)
        {
            return Find(proteinModel) ?? Create(proteinModel);
        }

        public Protein Find(ProteinModel proteinModel)
        {
            var protein = _dbContext.Proteins.FirstOrDefault(protein =>
                protein.Info.EnsemblId == proteinModel.EnsemblId
            );

            return protein;
        }

        public Protein Create(ProteinModel proteinModel)
        {
            var protein = Convert(proteinModel);

            _dbContext.Proteins.Add(protein);
            _dbContext.SaveChanges();

            return protein;
        }

        public IEnumerable<Protein> CreateMissing(IEnumerable<ProteinModel> proteinModels)
        {
            var proteinsToAdd = new List<Protein>();

            foreach (var proteinModel in proteinModels)
            {
                var protein = Find(proteinModel);

                if (protein == null)
                {
                    protein = Convert(proteinModel);

                    proteinsToAdd.Add(protein);
                }
            }

            if (proteinsToAdd.Any())
            {
                _dbContext.Proteins.AddRange(proteinsToAdd);
                _dbContext.SaveChanges();
            }

            return proteinsToAdd;
        }


        private Protein Convert(ProteinModel proteinModel)
        {
            var protein = new Protein
            {
                Start = proteinModel.Start,
                End = proteinModel.End,
                Length = proteinModel.Length,

                Info = new ProteinInfo
                {
                    EnsemblId = proteinModel.EnsemblId
                }
            };

            return protein;
        }
    }
}
