using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class MutationRepository
    {
        private readonly DomainDbContext _dbContext;


        public MutationRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Mutation FindOrCreate(MutationModel mutationModel)
        {
            return Find(mutationModel) ?? Create(mutationModel);
        }

        public Mutation Find(MutationModel mutationModel)
        {
            var mutation = _dbContext.Mutations.FirstOrDefault(mutation =>
                mutation.Code == mutationModel.Code
            );

            return mutation;
        }

        public Mutation Create(MutationModel mutationModel)
        {
            var mutation = new Mutation();

            Map(mutationModel, mutation);

            _dbContext.Mutations.Add(mutation);
            _dbContext.SaveChanges();

            return mutation;
        }

        public IEnumerable<Mutation> CreateMissing(IEnumerable<MutationModel> mutationModels)
        {
            var mutationsToAdd = new List<Mutation>();

            foreach (var mutationModel in mutationModels)
            {
                var mutation = Find(mutationModel);

                if (mutation == null)
                {
                    mutation = new Mutation();

                    Map(mutationModel, mutation);

                    mutationsToAdd.Add(mutation);
                }
            }

            if (mutationsToAdd.Any())
            {
                _dbContext.Mutations.AddRange(mutationsToAdd);
                _dbContext.SaveChanges();
            }

            return mutationsToAdd.ToArray();
        }


        private void Map(MutationModel mutationModel, Mutation mutation)
        {
            mutation.Code = mutationModel.Code;
            mutation.ChromosomeId = mutationModel.Chromosome;
            mutation.SequenceTypeId = mutationModel.SequenceType;
            mutation.Start = mutationModel.Start;
            mutation.End = mutationModel.End;
            mutation.ReferenceBase = mutationModel.ReferenceBase;
            mutation.AlternateBase = mutationModel.AlternateBase;
            mutation.TypeId = mutationModel.Type;
        }
    }
}
