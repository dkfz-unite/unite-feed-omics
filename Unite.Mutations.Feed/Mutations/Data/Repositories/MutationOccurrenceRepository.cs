using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Mutations.Data.Models;

namespace Unite.Mutations.Feed.Mutations.Data.Repositories
{
    internal class MutationOccurrenceRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly MutationRepository _mutationRepository;


        public MutationOccurrenceRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _mutationRepository = new MutationRepository(dbContext);
        }


        public MutationOccurrence FindOrCreate(int analysedSampleId, MutationModel mutationModel)
        {
            return Find(analysedSampleId, mutationModel) ?? Create(analysedSampleId, mutationModel);
        }

        public MutationOccurrence Find(int analysedSampleId, MutationModel mutationModel)
        {
            var mutation = _mutationRepository.Find(mutationModel);

            if (mutation != null)
            {
                return Find(analysedSampleId, mutation.Id);
            }

            return null;
        }

        public MutationOccurrence Create(int analysedSampleId, MutationModel mutationModel)
        {
            var mutation = _mutationRepository.FindOrCreate(mutationModel);

            return Create(analysedSampleId, mutation.Id);
        }

        public IEnumerable<MutationOccurrence> CreateMissing(int analysedSampleId, IEnumerable<MutationModel> mutationModels)
        {
            var mutationOccurrencesToAdd = new List<MutationOccurrence>();

            foreach (var mutationModel in mutationModels)
            {
                var mutation = _mutationRepository.FindOrCreate(mutationModel);

                var mutationOccurrence = Find(analysedSampleId, mutation.Id);

                if (mutationOccurrence == null)
                {
                    mutationOccurrence = Convert(analysedSampleId, mutation.Id);

                    mutationOccurrencesToAdd.Add(mutationOccurrence);
                }
            }

            if (mutationOccurrencesToAdd.Any())
            {
                _dbContext.MutationOccurrences.AddRange(mutationOccurrencesToAdd);
                _dbContext.SaveChanges();
            }

            return mutationOccurrencesToAdd;
        }


        private MutationOccurrence Find(int analysedSampleId, long mutationId)
        {
            var mutationOccurrence = _dbContext.MutationOccurrences.FirstOrDefault(mutationOccurrence =>
                mutationOccurrence.AnalysedSampleId == analysedSampleId &&
                mutationOccurrence.MutationId == mutationId
            );

            return mutationOccurrence;
        }

        private MutationOccurrence Create(int analysedSampleId, long mutationId)
        {
            var mutationOccurrence = Convert(analysedSampleId, mutationId);

            _dbContext.MutationOccurrences.Add(mutationOccurrence);
            _dbContext.SaveChanges();

            return mutationOccurrence;
        }

        private MutationOccurrence Convert(int analysedSampleId, long mutationId)
        {
            var mutationOccurrence = new MutationOccurrence();

            mutationOccurrence.AnalysedSampleId = analysedSampleId;
            mutationOccurrence.MutationId = mutationId;

            return mutationOccurrence;
        }
    }
}
