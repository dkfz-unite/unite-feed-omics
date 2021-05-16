using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Vep.Data.Models;

namespace Unite.Mutations.Annotations.Vep.Data.Repositories
{
    internal class MutationRepository
    {
        private readonly UniteDbContext _dbContext;


        public MutationRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Mutation Find(MutationModel mutationModel)
        {
            var mutation = Find(mutationModel.Code);

            return mutation;
        }

        public IEnumerable<Mutation> FindAll(IEnumerable<MutationModel> mutationModels)
        {
            var mutationCodes = mutationModels
                .Select(mutationModel => mutationModel.Code)
                .Distinct()
                .ToArray();

            var mutations = _dbContext.Mutations
                .Where(mutation => mutationCodes.Contains(mutation.Code))
                .ToArray();

            return mutations;
        }


        private Mutation Find(string code)
        {
            var mutation = _dbContext.Mutations.FirstOrDefault(mutation =>
                mutation.Code == code
            );

            return mutation;
        }
    }
}
