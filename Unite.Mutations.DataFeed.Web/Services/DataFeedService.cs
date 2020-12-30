using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Samples;
using Unite.Data.Services;
using Unite.Mutations.DataFeed.Domain.Resources.Samples.Extensions;
using Unite.Mutations.DataFeed.Web.Services.Audit;
using Unite.Mutations.DataFeed.Web.Services.Repositories;
using Unite.Mutations.DataFeed.Web.Services.Repositories.Tasks;

namespace Unite.Mutations.DataFeed.Web.Services
{
    public class DataFeedService : IDataFeedService
    {
        private readonly DonorRepository _donorRepository;
        private readonly SampleRepository _sampleRepository;
        private readonly SampleMutationRepository _sampleMutationRepository;
        private readonly MutationRepository _mutationRepository;
        private readonly DonorIndexingTaskRepository _donorIndexingTaskRepository;
        private readonly MutationIndexingTaskRepository _mutationIndexingTaskRepository;
        private readonly ILogger _logger;


        public DataFeedService(
            UniteDbContext database,
            ILogger<DataFeedService> logger)
        {
            _donorRepository = new DonorRepository(database, logger);
            _sampleRepository = new SampleRepository(database, logger);
            _sampleMutationRepository = new SampleMutationRepository(database, logger);
            _mutationRepository = new MutationRepository(database, logger);
            _donorIndexingTaskRepository = new DonorIndexingTaskRepository(database, logger);
            _mutationIndexingTaskRepository = new MutationIndexingTaskRepository(database, logger);
            _logger = logger;
        }


        public void ProcessSamples(IEnumerable<Domain.Resources.Samples.Sample> samples)
        {
            var totalMutations = samples.SelectMany(sample => sample.Mutations).Count();

            _logger.LogInformation($"Processing {totalMutations} mutations");

            var donors = new HashSet<string>();
            var mutations = new HashSet<int>();
            var audit = new UploadAudit();

            foreach (var sampleResource in samples)
            {
                sampleResource.Sanitise();


                var donorModel = sampleResource.GetDonor();
                var donor = GetOrCreate(donorModel, ref audit);


                var sampleModel = sampleResource.GetSample();
                var sample = GetOrCreate(sampleModel, ref audit);


                foreach(var mutationResource in sampleResource.Mutations)
                {
                    var mutationModel = mutationResource.GetMutation();
                    var mutation = CreateOrUpdate(mutationModel, ref audit);


                    var sampleMutationModel = mutationResource.GetSampleMutation();
                    sampleMutationModel.Sample = sample;
                    sampleMutationModel.Mutation = mutation;
                    
                    var sampleMutation = CreateOrUpdate(sampleMutationModel, ref audit);


                    mutations.Add(mutation.Id);
                }


                donors.Add(donor.Id);
            }

            _mutationIndexingTaskRepository.AddRange(mutations.ToArray());
            _donorIndexingTaskRepository.AddRange(donors.ToArray());

            _logger.LogInformation(audit.ToString());
        }


        private Donor GetOrCreate(in Donor donor, ref UploadAudit audit)
        {
            var entity = _donorRepository.Find(donor.Id);

            if (entity == null)
            {
                entity = _donorRepository.Add(donor);

                audit.DonorsCreated++;
            }

            return entity;
        }

        private Sample GetOrCreate(in Sample sample, ref UploadAudit audit)
        {
            var entity = _sampleRepository.Find(sample.Donor.Id, sample.Name, sample.TypeId, sample.SubtypeId);

            if (entity == null)
            {
                entity = _sampleRepository.Add(sample);

                audit.SamplesCreated++;
            }

            return entity;
        }

        private Mutation CreateOrUpdate(in Mutation mutation, ref UploadAudit audit)
        {
            var entity = _mutationRepository.Find(mutation.Code);

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

        private SampleMutation CreateOrUpdate(in SampleMutation sampleMutation, ref UploadAudit audit)
        {
            var entity = _sampleMutationRepository.Find(sampleMutation.Sample.Id, sampleMutation.Mutation.Id);

            if (entity == null)
            {
                entity = _sampleMutationRepository.Add(sampleMutation);

                audit.MutationsAssociated++;
            }
            else
            {
                _sampleMutationRepository.Update(ref entity, sampleMutation);
            }

            return entity;
        }
    }
}
