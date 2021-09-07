using System;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;
using Unite.Mutations.Feed.Data.Mutations.Models.Enums;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class SpecimenRepository
    {
        private readonly DomainDbContext _dbContext;
        private readonly DonorRepository _donorRepository;
        private readonly TissueRepository _tissueRepository;
        private readonly CellLineRepository _cellLineRepository;
        private readonly OrganoidRepository _organoidRepository;
        private readonly XenograftRepository _xenograftRepository;


        public SpecimenRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _donorRepository = new DonorRepository(dbContext);
            _tissueRepository = new TissueRepository(dbContext);
            _cellLineRepository = new CellLineRepository(dbContext);
            _organoidRepository = new OrganoidRepository(dbContext);
            _xenograftRepository = new XenograftRepository(dbContext);
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

            if (specimenModel.Type == SpecimenType.Tissue)
            {
                return _tissueRepository.Find(donor.Id, specimenModel.ReferenceId);
            }
            else if (specimenModel.Type == SpecimenType.CellLine)
            {
                return _cellLineRepository.Find(donor.Id, specimenModel.ReferenceId);
            }
            else if (specimenModel.Type == SpecimenType.Organoid)
            {
                return _organoidRepository.Find(donor.Id, specimenModel.ReferenceId);
            }
            else if (specimenModel.Type == SpecimenType.Xenograft)
            {
                return _xenograftRepository.Find(donor.Id, specimenModel.ReferenceId);
            }
            else
            {
                throw new NotSupportedException("Specimen type is not supported");
            }
        }

        public Specimen Create(SpecimenModel specimenModel)
        {
            var donor = _donorRepository.FindOrCreate(specimenModel.Donor);

            if (specimenModel.Type == SpecimenType.Tissue)
            {
                return _tissueRepository.Create(donor.Id, specimenModel.ReferenceId);
            }
            else if (specimenModel.Type == SpecimenType.CellLine)
            {
                return _cellLineRepository.Create(donor.Id, specimenModel.ReferenceId);
            }
            else if (specimenModel.Type == SpecimenType.Organoid)
            {
                return _organoidRepository.Create(donor.Id, specimenModel.ReferenceId);
            }
            else if (specimenModel.Type == SpecimenType.Xenograft)
            {
                return _xenograftRepository.Create(donor.Id, specimenModel.ReferenceId);
            }
            else
            {
                throw new NotSupportedException("Specimen type is not supported");
            }
        }
    }
}
