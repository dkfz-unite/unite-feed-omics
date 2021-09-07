using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Mutations.Indices.Services.Mappers;

namespace Unite.Mutations.Indices.Services
{
    public class MutationIndexCreationService : IIndexCreationService<MutationIndex>
    {
        private readonly DomainDbContext _dbContext;
        private readonly MutationIndexMapper _mutationIndexMapper;
        private readonly DonorIndexMapper _donorIndexMapper;
        private readonly SpecimenIndexMapper _specimenIndexMapper;


        public MutationIndexCreationService(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _mutationIndexMapper = new MutationIndexMapper();
            _donorIndexMapper = new DonorIndexMapper();
            _specimenIndexMapper = new SpecimenIndexMapper();
        }


        public MutationIndex CreateIndex(object key)
        {
            var mutationId = (long)key;

            return CreateMutationIndex(mutationId);
        }


        private MutationIndex CreateMutationIndex(long mutationId)
        {
            var mutation = LoadMutation(mutationId);

            var index = CreateMutationIndex(mutation);

            return index;
        }

        private MutationIndex CreateMutationIndex(Mutation mutation)
        {
            if (mutation == null)
            {
                return null;
            }

            var index = new MutationIndex();

            _mutationIndexMapper.Map(mutation, index);

            index.Donors = CreateDonorIndices(mutation.Id);

            index.NumberOfDonors = index.Donors
                .Select(donor => donor.Id)
                .Distinct()
                .Count();

            index.NumberOfSpecimens = index.Donors
                .SelectMany(donor => donor.Specimens)
                .Select(specimen => specimen.Id)
                .Distinct()
                .Count();

            return index;
        }

        private Mutation LoadMutation(long mutationId)
        {
            var mutation = _dbContext.Mutations
                .IncludeAffectedTranscripts()
                .FirstOrDefault(mutation => mutation.Id == mutationId);

            return mutation;
        }


        private DonorIndex[] CreateDonorIndices(long mutationId)
        {
            var donors = LoadDonors(mutationId);

            if (donors == null)
            {
                return null;
            }

            var indices = donors
                .Select(donor => CreateDonorIndex(donor, mutationId))
                .ToArray();

            return indices;
        }

        private DonorIndex CreateDonorIndex(Donor donor, long mutationId)
        {
            var index = new DonorIndex();

            _donorIndexMapper.Map(donor, index);

            index.Specimens = CreateSpecimenIndices(donor.Id, mutationId);

            return index;
        }

        private Donor[] LoadDonors(long mutationId)
        {
            var donorIds = _dbContext.MutationOccurrences
                .Where(mutationOccurrence => mutationOccurrence.MutationId == mutationId)
                .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId)
                .Distinct()
                .ToArray();

            var donors = _dbContext.Donors
                .IncludeClinicalData()
                .IncludeTreatments()
                .IncludeWorkPackages()
                .IncludeStudies()
                .Where(donor => donorIds.Contains(donor.Id))
                .ToArray();

            return donors;
        }


        private SpecimenIndex[] CreateSpecimenIndices(int donorId, long mutationId)
        {
            var specimens = LoadSpecimens(donorId, mutationId);

            if (specimens == null)
            {
                return null;
            }

            var indices = specimens
                .Select(CreateSpecimenIndex)
                .ToArray();

            return indices;
        }

        private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
        {
            var index = new SpecimenIndex();

            _specimenIndexMapper.Map(specimen, index);

            return index;
        }

        private Specimen[] LoadSpecimens(int donorId, long mutationId)
        {
            var specimenIds = _dbContext.MutationOccurrences
                .Where(mutationOccurrence =>
                    mutationOccurrence.MutationId == mutationId &&
                    mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId == donorId
                )
                .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.SpecimenId)
                .Distinct()
                .ToArray();

            var specimens = _dbContext.Specimens
                .IncludeTissue()
                .IncludeCellLine()
                .IncludeOrganoid()
                .IncludeXenograft()
                .IncludeMolecularData()
                .Where(specimen => specimenIds.Contains(specimen.Id))
                .ToArray();

            return specimens;
        }
    }
}
