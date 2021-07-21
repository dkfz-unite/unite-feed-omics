using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;
using Unite.Mutations.Feed.Data.Mutations.Models.Audit;
using Unite.Mutations.Feed.Data.Mutations.Repositories;

namespace Unite.Mutations.Feed.Data.Mutations
{
    public class MutationDataWriter : DataWriter<AnalysisModel, MutationsUploadAudit>
    {
        private readonly AnalysisRepository _analysisRepository;
        private readonly AnalysedSampleRepository _analysedSampleRepository;
        private readonly MutationRepository _mutationRepository;
        private readonly MutationOccurrenceRepository _mutationOccurrenceRepository;


        public MutationDataWriter(UniteDbContext dbContext) : base(dbContext)
        {
            _analysisRepository = new AnalysisRepository(dbContext);
            _analysedSampleRepository = new AnalysedSampleRepository(dbContext);
            _mutationRepository = new MutationRepository(dbContext);
            _mutationOccurrenceRepository = new MutationOccurrenceRepository(dbContext);
        }


        protected override void ProcessModel(AnalysisModel analysisModel, ref MutationsUploadAudit audit)
        {
            var analysis = _analysisRepository.FindOrCreate(analysisModel);

            foreach (var analysedSampleModel in analysisModel.AnalysedSamples)
            {
                var analysedSample = _analysedSampleRepository.FindOrCreate(analysis.Id, analysedSampleModel);

                if (analysedSampleModel.Mutations != null)
                {
                    var mutations = _mutationRepository.CreateMissing(analysedSampleModel.Mutations);

                    foreach (var mutation in mutations)
                    {
                        audit.Mutations.Add(mutation.Id);

                        audit.MutationsCreated++;
                    }

                    var mutationOccurrences = _mutationOccurrenceRepository.CreateMissing(analysedSample.Id, analysedSampleModel.Mutations);

                    foreach (var mutationOccurrence in mutationOccurrences)
                    {
                        audit.MutationOccurrences.Add(mutationOccurrence.MutationId);

                        audit.MutationsAssociated++;
                    }
                }
            }
        }
    }
}
