using System;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class SpecimenRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly DonorRepository _donorRepository;
        private readonly TissueRepository _tissueRepository;
        private readonly CellLineRepository _cellLineRepository;


        public SpecimenRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _donorRepository = new DonorRepository(dbContext);
            _tissueRepository = new TissueRepository(dbContext);
            _cellLineRepository = new CellLineRepository(dbContext);
        }


        public Specimen FindOrCreate(SpecimenModel specimenModel)
        {
            return Find(specimenModel) ?? Create(specimenModel);
        }

        public Specimen Find(SpecimenModel specimenModel)
        {
            var donor = _donorRepository.Find(specimenModel.Donor);

            if (donor == null)
            {
                return null;
            }

            if (specimenModel is TissueModel tissueModel)
            {
                return _tissueRepository.Find(donor.Id, tissueModel);
            }
            else if (specimenModel is CellLineModel cellLineModel)
            {
                return _cellLineRepository.Find(donor.Id, cellLineModel);
            }
            else
            {
                throw new NotSupportedException("Specimen type is not supported");
            }
        }

        public Specimen Create(SpecimenModel specimenModel)
        {
            var donor = _donorRepository.FindOrCreate(specimenModel.Donor);

            if (specimenModel is TissueModel tissueModel)
            {
                return _tissueRepository.Create(donor.Id, tissueModel);
            }
            else if (specimenModel is CellLineModel cellLineModel)
            {
                return _cellLineRepository.Create(donor.Id, cellLineModel);
            }
            else
            {
                throw new NotSupportedException("Specimen type is not supported");
            }
        }
    }
}
