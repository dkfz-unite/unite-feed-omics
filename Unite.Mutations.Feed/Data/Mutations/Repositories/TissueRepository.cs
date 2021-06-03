using System.Linq;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class TissueRepository
    {
        private readonly UniteDbContext _dbContext;


        public TissueRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Specimen FindOrCreate(int donorId, TissueModel tissueModel)
        {
            return Find(donorId, tissueModel) ?? Create(donorId, tissueModel);
        }

        public Specimen Find(int donorId, TissueModel tissueModel)
        {
            var specimen = _dbContext.Specimens.FirstOrDefault(specimen =>
                specimen.DonorId == donorId &&
                specimen.Tissue.ReferenceId == tissueModel.ReferenceId
            );

            return specimen;
        }

        public Specimen Create(int donorId, TissueModel tissueModel)
        {
            var specimen = new Specimen
            {
                DonorId = donorId
            };

            Map(tissueModel, specimen);

            _dbContext.Specimens.Add(specimen);
            _dbContext.SaveChanges();

            return specimen;
        }


        private void Map(TissueModel tissueModel, Specimen specimen)
        {
            if (specimen.Tissue == null)
            {
                specimen.Tissue = new Tissue();
            }

            specimen.Tissue.ReferenceId = tissueModel.ReferenceId;
            specimen.Tissue.TypeId = tissueModel.Type;
            specimen.Tissue.TumourTypeId = tissueModel.TumourType;
            specimen.Tissue.ExtractionDate = tissueModel.ExtractionDate;
            specimen.Tissue.Source = GetTissueSource(tissueModel.Source);
        }

        private TissueSource GetTissueSource(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var tissueSource = _dbContext.TissueSources.FirstOrDefault(tissueSource =>
                tissueSource.Value == value
            );

            if (tissueSource == null)
            {
                tissueSource = new TissueSource { Value = value };

                _dbContext.TissueSources.Add(tissueSource);
                _dbContext.SaveChanges();
            }

            return tissueSource;
        }
    }
}
