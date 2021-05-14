using System;
using System.Linq;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues;
using Unite.Data.Services;
using Unite.Mutations.Feed.Mutations.Data.Models;

namespace Unite.Mutations.Feed.Mutations.Data.Repositories
{
    internal class SpecimenRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly DonorRepository _donorRepository;


        public SpecimenRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _donorRepository = new DonorRepository(dbContext);
        }


        public Specimen FindOrCreate(SpecimenModel specimenModel)
        {
            return Find(specimenModel) ?? Create(specimenModel);
        }

        public Specimen Find(SpecimenModel specimenModel)
        {
            if (!string.IsNullOrWhiteSpace(specimenModel.ReferenceId))
            {
                return Find(specimenModel.ReferenceId);
            }
            else
            {
                var donor = _donorRepository.Find(specimenModel.Donor);

                if(donor != null)
                {
                    if (specimenModel.Tissue != null)
                    {
                        return Find(donor.Id, specimenModel.Tissue);
                    }
                }
            }

            return null;
        }

        public Specimen Create(SpecimenModel specimenModel)
        {
            var donor = specimenModel.Donor == null ? null : _donorRepository.FindOrCreate(specimenModel.Donor);

            if(specimenModel.Tissue != null)
            {
                return Create(donor?.Id, specimenModel.ReferenceId, specimenModel.Tissue);
            }
            else
            {
                throw new NotImplementedException("Specimen type is not supported");
            }
        }


        private Specimen Find(string referenceId)
        {
            var specimen = _dbContext.Specimens.FirstOrDefault(specimen =>
                specimen.ReferenceId == referenceId
            );

            return specimen;
        }

        private Specimen Find(int donorId, TissueModel tissueModel)
        {
            var specimen = _dbContext.Specimens.FirstOrDefault(specimen =>
                specimen.DonorId == donorId &&
                specimen.Tissue != null &&
                specimen.Tissue.TypeId == tissueModel.Type &&
                specimen.Tissue.TumourTypeId == tissueModel.TumourType
            );

            return specimen;
        }

        private Specimen Create(int? donorId, string referenceId, TissueModel tissueModel)
        {
            var specimen = new Specimen();

            specimen.DonorId = donorId;

            specimen.ReferenceId = referenceId;

            specimen.Tissue = new Tissue
            {
                TypeId = tissueModel.Type,
                TumourTypeId = tissueModel.TumourType
            };

            _dbContext.Specimens.Add(specimen);
            _dbContext.SaveChanges();

            return specimen;
        }
    }
}
