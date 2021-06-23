using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Mutations.Indices.Services.Mappers;

namespace Unite.Mutations.Indices.Services
{
    public class MutationIndexCreationService : IIndexCreationService<MutationIndex>
    {
        private readonly UniteDbContext _dbContext;
        private readonly MutationIndexMapper _mutationIndexMapper;
        private readonly DonorIndexMapper _donorIndexMapper;
        private readonly SpecimenIndexMapper _specimenIndexMapper;

        public MutationIndexCreationService(UniteDbContext dbContext)
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
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Gene)
                        .ThenInclude(gene => gene.Info)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Gene)
                        .ThenInclude(gene => gene.Biotype)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Transcript)
                        .ThenInclude(transcript => transcript.Info)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Transcript)
                        .ThenInclude(transcript => transcript.Biotype)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Consequences)
                        .ThenInclude(affectedTranscriptConsequence => affectedTranscriptConsequence.Consequence)
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
                .Include(donor => donor.ClinicalData)
                    .ThenInclude(clinicalData => clinicalData.PrimarySite)
                .Include(donor => donor.ClinicalData)
                    .ThenInclude(clinicalData => clinicalData.Localization)
                .Include(donor => donor.Treatments)
                    .ThenInclude(treatment => treatment.Therapy)
                .Include(donor => donor.DonorWorkPackages)
                    .ThenInclude(workPackageDonor => workPackageDonor.WorkPackage)
                .Include(donor => donor.DonorStudies)
                    .ThenInclude(studyDonor => studyDonor.Study)
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
                .Include(specimen => specimen.Tissue)
                    .ThenInclude(tissue => tissue.Source)
                .Include(specimen => specimen.CellLine)
                    .ThenInclude(cellLine => cellLine.Info)
                .Include(specimen => specimen.Organoid)
                    .ThenInclude(organoid => organoid.Interventions)
                        .ThenInclude(intervention => intervention.Type)
                .Include(specimen => specimen.Xenograft)
                    .ThenInclude(xenograft => xenograft.Interventions)
                        .ThenInclude(intervention => intervention.Type)
                .Include(specimen => specimen.MolecularData)
                .Where(specimen => specimenIds.Contains(specimen.Id))
                .ToArray();

            return specimens;
        }
    }
}
