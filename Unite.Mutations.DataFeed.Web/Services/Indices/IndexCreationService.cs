using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities;
using Unite.Data.Entities.Cells;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Extensions;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Samples;
using Unite.Data.Services;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Mutations;

namespace Unite.Mutations.DataFeed.Web.Services.Indices
{
    public class IndexCreationService : IIndexCreationService
    {
        private readonly UniteDbContext _database;

        public IndexCreationService(UniteDbContext database)
        {
            _database = database;
        }

        public MutationIndex CreateIndex(int mutationId)
        {
            var mutation = LoadMutation(mutationId);

            return CreateMutationIndex(mutation);
        }


        private Mutation LoadMutation(int mutationId)
        {
            var mutation = _database.Mutations
                .Include(mutation => mutation.Gene)
                .Include(mutation => mutation.Contig)
                .FirstOrDefault(mutation => mutation.Id == mutationId);

            if (mutation != null)
            {
                mutation.MutationSamples = _database.SampleMutations
                    .Include(sampleMutation => sampleMutation.Sample)
                    .Include(sampleMutation => sampleMutation.Sample.Donor)
                    .Include(sampleMutation => sampleMutation.Sample.Donor.PrimarySite)
                    .Include(sampleMutation => sampleMutation.Sample.Donor.ClinicalData)
                        .ThenInclude(clinicalData => clinicalData.Localization)
                    .Include(sampleMutation => sampleMutation.Sample.Donor.Treatments)
                        .ThenInclude(treatment => treatment.Therapy)
                    .Include(sampleMutation => sampleMutation.Sample.Donor.DonorWorkPackages)
                        .ThenInclude(donorWorkPackage => donorWorkPackage.WorkPackage)
                    .Include(sampleMutation => sampleMutation.Sample.Donor.DonorStudies)
                        .ThenInclude(donorStudy => donorStudy.Study)
                    .Include(sampleMutation => sampleMutation.Sample.CellLine)
                    .Include(sampleMutation => sampleMutation.Sample.CellLine.Parent)
                    .Include(sampleMutation => sampleMutation.Sample.CellLine.Childern)
                    .Where(sampleMutation => sampleMutation.MutationId == mutation.Id)
                    .ToArray();
            }

            return mutation;
        }


        private MutationIndex CreateMutationIndex(in Mutation mutation)
        {
            if (mutation == null)
            {
                return null;
            }

            var index = new MutationIndex();

            index.Id = mutation.Id;
            index.Code = mutation.Code;
            index.Name = mutation.Name;
            index.Chromosome = mutation.ChromosomeId?.ToDefinitionString();
            index.Contig = mutation.Contig?.Value;
            index.SequenceType = mutation.SequenceTypeId.ToDefinitionString();
            index.Position = mutation.Position;
            index.Type = mutation.TypeId.ToDefinitionString();

            index.Gene = CreateGeneIndex(mutation.Gene);
            index.Samples = CreateSampleIndices(mutation.MutationSamples);

            return index;
        }

        private GeneIndex CreateGeneIndex(in Gene gene)
        {
            if (gene == null)
            {
                return null;
            }

            var index = new GeneIndex();

            index.Id = gene.Id;
            index.Name = gene.Name;

            return index;
        }


        private SampleIndex[] CreateSampleIndices(in IEnumerable<SampleMutation> samples)
        {
            if (samples == null)
            {
                return null;
            }

            var indices = samples.Select(sample =>
            {
                var index = new SampleIndex();

                index.Id = sample.Sample.Id;
                index.Name = sample.Sample.Name;
                index.Link = sample.Sample.Link;
                index.Type = sample.Sample.TypeId?.ToDefinitionString();
                index.Subtype = sample.Sample.SubtypeId?.ToDefinitionString();

                index.Donor = CreateDonorIndex(sample.Sample.Donor);
                index.CellLine = CreateCellLineIndex(sample.Sample.CellLine);

                return index;
            })
            .ToArray();

            return indices;
        }


        private CellLineIndex CreateCellLineIndex(in CellLine cellLine)
        {
            if (cellLine == null)
            {
                return null;
            }

            var index = new CellLineIndex();

            index.Id = cellLine.Id;
            index.Name = cellLine.Name;
            index.Type = cellLine.TypeId?.ToDefinitionString();
            index.Species = cellLine.SpeciesId?.ToDefinitionString();
            index.GeneExpressionSubtype = cellLine.GeneExpressionSubtypeId?.ToDefinitionString();
            index.IdhStatus = cellLine.IdhStatusId?.ToDefinitionString();
            index.IdhMutation = cellLine.IdhMutationId?.ToDefinitionString();
            index.MethylationStatus = cellLine.MethylationStatusId?.ToDefinitionString();
            index.MethylationSubtype = cellLine.MethylationSubtypeId?.ToDefinitionString();
            index.GcimpMethylation = cellLine.GcimpMethylation;

            index.Parent = CreateCellLineBaseIndex(cellLine.Parent);
            index.Children = CreateCellLineBaseIndices(cellLine.Childern);

            return index;
        }

        private CellLineBaseIndex CreateCellLineBaseIndex(in CellLine cellLine)
        {
            if (cellLine == null)
            {
                return null;
            }

            var index = new CellLineBaseIndex();

            index.Id = cellLine.Id;
            index.Name = cellLine.Name;
            index.Type = cellLine.TypeId?.ToDefinitionString();
            index.Species = cellLine.SpeciesId?.ToDefinitionString();
            index.GeneExpressionSubtype = cellLine.GeneExpressionSubtypeId?.ToDefinitionString();
            index.IdhStatus = cellLine.IdhStatusId?.ToDefinitionString();
            index.IdhMutation = cellLine.IdhMutationId?.ToDefinitionString();
            index.MethylationStatus = cellLine.MethylationStatusId?.ToDefinitionString();
            index.MethylationSubtype = cellLine.MethylationSubtypeId?.ToDefinitionString();
            index.GcimpMethylation = cellLine.GcimpMethylation;

            return index;
        }

        private CellLineBaseIndex[] CreateCellLineBaseIndices(in IEnumerable<CellLine> cellLines)
        {
            if (cellLines == null)
            {
                return null;
            }

            var indices = cellLines.Select(cellLine =>
            {
                var index = CreateCellLineBaseIndex(cellLine);

                return index;
            })
            .ToArray();

            return indices;
        }


        private DonorIndex CreateDonorIndex(in Donor donor)
        {
            if (donor == null)
            {
                return null;
            }

            var index = new DonorIndex();

            index.Id = donor.Id;
            index.Diagnosis = donor.Diagnosis;
            index.DiagnosisDate = donor.DiagnosisDate;
            index.PrimarySite = donor.PrimarySite?.Value;
            index.Origin = donor.Origin;
            index.MtaProtected = donor.MtaProtected;

            index.ClinicalData = CreateClinicalDataIndex(donor.ClinicalData);
            index.Treatments = CreateTreatmentIndices(donor.Treatments);
            index.WorkPackages = CreateWorkPackageIndices(donor.DonorWorkPackages);
            index.Studies = CreateStudyIndices(donor.DonorStudies);

            return index;
        }


        private ClinicalDataIndex CreateClinicalDataIndex(in ClinicalData clinicalData)
        {
            if (clinicalData == null)
            {
                return null;
            }

            var index = new ClinicalDataIndex();

            index.Gender = clinicalData.GenderId?.ToDefinitionString();
            index.Age = clinicalData.Age;
            index.AgeCategory = clinicalData.AgeCategoryId?.ToDefinitionString();
            index.Localization = clinicalData.Localization?.Value;
            index.VitalStatus = clinicalData.VitalStatusId?.ToDefinitionString();
            index.VitalStatusChangeDate = clinicalData.VitalStatusChangeDate;
            index.SurvivalDays = clinicalData.SurvivalDays;
            index.ProgressionDate = clinicalData.ProgressionDate;
            index.ProgressionFreeDays = clinicalData.ProgressionFreeDays;
            index.RelapseDate = clinicalData.RelapseDate;
            index.RelapseFreeDays = clinicalData.RelapseFreeDays;
            index.KpsBaseline = clinicalData.KpsBaseline;
            index.SteroidsBaseline = clinicalData.SteroidsBaseline;

            return index;
        }


        private TreatmentIndex[] CreateTreatmentIndices(in IEnumerable<Treatment> treatments)
        {
            if (treatments == null)
            {
                return null;
            }

            var indices = treatments.Select(treatment =>
            {
                var index = new TreatmentIndex();

                index.Therapy = CreateTherapyIndex(treatment.Therapy);
                index.Details = treatment.Details;
                index.Results = treatment.Results;

                return index;
            })
            .ToArray();

            return indices;
        }

        private TherapyIndex CreateTherapyIndex(in Therapy therapy)
        {
            if(therapy == null)
            {
                return null;
            }

            var index = new TherapyIndex();

            index.Id = therapy.Id;
            index.Name = therapy.Name;

            return index;
        }


        private WorkPackageIndex[] CreateWorkPackageIndices(in IEnumerable<WorkPackageDonor> workPackages)
        {
            if (workPackages == null)
            {
                return null;
            }

            var indices = workPackages.Select(workPackage =>
            {
                var index = new WorkPackageIndex();

                index.Id = workPackage.WorkPackage.Id;
                index.Name = workPackage.WorkPackage.Name;

                return index;
            })
            .ToArray();

            return indices;
        }


        private StudyIndex[] CreateStudyIndices(in IEnumerable<StudyDonor> studies)
        {
            if(studies == null)
            {
                return null;
            }

            var indices = studies.Select(study =>
            {
                var index = new StudyIndex();

                index.Id = study.Study.Id;
                index.Name = study.Study.Name;

                return index;
            })
            .ToArray();

            return indices;
        }
    }
}
