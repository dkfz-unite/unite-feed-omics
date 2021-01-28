using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.DataFeed.Domain.Resources.Mutations.Extensions;
using Unite.Mutations.DataFeed.Web.Services.Audit;
using Unite.Mutations.DataFeed.Web.Services.Repositories;
using Unite.Mutations.DataFeed.Web.Services.Repositories.Tasks;

namespace Unite.Mutations.DataFeed.Web.Services
{
    public class DataFeedService : IDataFeedService<Domain.Resources.Mutations.Resource>
    {
        private readonly UniteDbContext _database;
        private readonly Repository<Donor> _donorRepository;
        private readonly Repository<Analysis> _analysisRepository;
        private readonly Repository<Sample> _sampleRepository;
        private readonly Repository<AnalysedSample> _analysedSampleRepository;
        private readonly Repository<MatchedSample> _matchedSampleRepository;
        private readonly Repository<Mutation> _mutationRepository;
        private readonly Repository<MutationOccurrence> _mutationOccurrenceRepository;
        private readonly DonorIndexingTaskRepository _donorIndexingTaskRepository;
        private readonly MutationIndexingTaskRepository _mutationIndexingTaskRepository;
        private readonly ILogger _logger;


        public DataFeedService(
            UniteDbContext database,
            ILogger<DataFeedService> logger)
        {
            _database = database;
            _donorRepository = new DonorRepository(database, logger);
            _analysisRepository = new AnalysisRepository(database, logger);
            _sampleRepository = new SampleRepository(database, logger);
            _analysedSampleRepository = new AnalysedSampleRepository(database, logger);
            _matchedSampleRepository = new MatchedSampleRepository(database, logger);
            _mutationRepository = new MutationRepository(database, logger);
            _mutationOccurrenceRepository = new MutationOccurrenceRepository(database, logger);
            _donorIndexingTaskRepository = new DonorIndexingTaskRepository(database, logger);
            _mutationIndexingTaskRepository = new MutationIndexingTaskRepository(database, logger);
            _logger = logger;
        }


        public void ProcessResources(IEnumerable<Domain.Resources.Mutations.Resource> resources)
        {
            var totalMutations = resources
                .SelectMany(
                    resource => resource.Samples
                        .Where(sample => sample.Mutations != null)
                        .SelectMany(sample => sample.Mutations)
                ).Count();

            _logger.LogInformation($"Processing {totalMutations} mutations");

            var donors = new HashSet<string>();
            var mutations = new HashSet<int>();
            var audit = new UploadAudit();

            foreach (var resource in resources)
            {
                resource.Sanitise();

                var transaction = _database.Database.BeginTransaction();

                var donorModel = resource.GetDonor();
                var donor = GetOrCreate(donorModel, ref audit);

                var analysisModel = resource.GetAnalysis(resource.Pid);
                var analysis = CreateOrUpdate(analysisModel, ref audit);

                foreach(var sampleResource in resource.Samples)
                {
                    var sampleModel = sampleResource.GetSample(resource.Pid);
                    var sample = CreateOrUpdate(sampleModel, ref audit);

                    var analysedSampleModel = new AnalysedSample() { Analysis = analysis, Sample = sample };
                    var analysedSample = GetOrCreate(analysedSampleModel, ref audit);

                    if(sampleResource.MatchedSamples != null)
                    {
                        foreach (var matchingSampleName in sampleResource.MatchedSamples)
                        {
                            var matchingSampleResource = resource.Samples.Single(sample => sample.Name == matchingSampleName);

                            var matchingSampleModel = matchingSampleResource.GetSample(resource.Pid);
                            var matchingSample = CreateOrUpdate(matchingSampleModel, ref audit);

                            var analysedMatchingSampleModel = new AnalysedSample() { Analysis = analysis, Sample = matchingSample };
                            var analysedMatchingSample = GetOrCreate(analysedMatchingSampleModel, ref audit);

                            var matchedSampleModel = new MatchedSample() { Analysed = analysedSample, Matched = analysedMatchingSample };
                            var matchedSample = GetOrCreate(matchedSampleModel, ref audit);
                        }
                    }

                    if(sampleResource.Mutations != null)
                    {
                        foreach (var mutationResource in sampleResource.Mutations)
                        {
                            var mutationModel = mutationResource.GetMutation();
                            var mutation = CreateOrUpdate(mutationModel, ref audit);

                            var mutationOccurrenceModel = new MutationOccurrence() { AnalysedSample = analysedSample, Mutation = mutation };
                            var mutationOccurrence = GetOrCreate(mutationOccurrenceModel, ref audit);

                            mutations.Add(mutation.Id);
                        }
                    }
                }

                donors.Add(donor.Id);

                transaction.Commit();
            }

            _mutationIndexingTaskRepository.AddRange(mutations.ToArray());

            _donorIndexingTaskRepository.AddRange(donors.ToArray());

            _logger.LogInformation(audit.ToString());
        }


        private Donor GetOrCreate(in Donor donor, ref UploadAudit audit)
        {
            var id = donor.Id;

            var entity = _donorRepository.Find(entity =>
                entity.Id == id
            );

            if (entity == null)
            {
                entity = _donorRepository.Add(donor);

                audit.DonorsCreated++;
            }

            return entity;
        }

        private Analysis CreateOrUpdate(in Analysis analysis, ref UploadAudit audit)
        {
            var donorId = analysis.DonorId;
            var name = analysis.Name;

            var entity = _analysisRepository.Find(entity =>
                entity.DonorId == donorId &&
                entity.Name == name
            );

            if(entity == null)
            {
                entity = _analysisRepository.Add(analysis);

                audit.AnalysesCreated++;
            }
            else
            {
                _analysisRepository.Update(ref entity, analysis);
            }

            return entity;
        }

        private Sample CreateOrUpdate(in Sample sample, ref UploadAudit audit)
        {
            var donorId = sample.DonorId;
            var name = sample.Name;

            var entity = _sampleRepository.Find(entity =>
                entity.DonorId == donorId &&
                entity.Name == name
            );

            if (entity == null)
            {
                entity = _sampleRepository.Add(sample);

                audit.SamplesCreated++;
            }
            else
            {
                _sampleRepository.Update(ref entity, sample);
            }

            return entity;
        }

        private AnalysedSample GetOrCreate(in AnalysedSample analysedSample, ref UploadAudit audit)
        {
            var analysisId = analysedSample.Analysis.Id;
            var sampleId = analysedSample.Sample.Id;

            var entity = _analysedSampleRepository.Find(entity =>
                entity.AnalysisId == analysisId &&
                entity.SampleId == sampleId
            );

            if(entity == null)
            {
                entity = _analysedSampleRepository.Add(analysedSample);

                audit.SamplesAnalysed++;
            }

            return entity;
        } 

        private MatchedSample GetOrCreate(in MatchedSample matchedSample, ref UploadAudit audit)
        {
            var analysedSampleId = matchedSample.Analysed.Id;
            var matchedSampleId = matchedSample.Matched.Id;

            var entity = _matchedSampleRepository.Find(entity =>
                entity.AnalysedSampleId == analysedSampleId &&
                entity.MatchedSampleId == matchedSampleId
            );

            if(entity == null)
            {
                entity = _matchedSampleRepository.Add(matchedSample);

                audit.SamplesMatched++;
            }

            return entity;
        }

        private Mutation CreateOrUpdate(in Mutation mutation, ref UploadAudit audit)
        {
            var code = mutation.Code;

            var entity = _mutationRepository.Find(entity =>
                entity.Code == code
            );

            if (entity == null)
            {
                entity = _mutationRepository.Add(mutation);

                audit.MutationsCreated++;
            }
            else
            {
                _mutationRepository.Update(ref entity, mutation);
            }

            return entity;
        }

        private MutationOccurrence GetOrCreate(in MutationOccurrence mutationOccurrence, ref UploadAudit audit)
        {
            var analysedSampleId = mutationOccurrence.AnalysedSample.Id;
            var mutationId = mutationOccurrence.Mutation.Id;

            var entity = _mutationOccurrenceRepository.Find(entity =>
                entity.AnalysedSampleId == analysedSampleId &&
                entity.MutationId == mutationId
            );

            if(entity == null)
            {
                entity = _mutationOccurrenceRepository.Add(mutationOccurrence);

                audit.MutationsAssociated++;
            }

            return entity;
        }
    }
}
