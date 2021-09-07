using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Data.Models;

namespace Unite.Mutations.Annotations.Data.Repositories
{
    internal class MutationRepository
    {
        private readonly DomainDbContext _dbContext;


        public MutationRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Mutation Find(MutationModel mutationModel)
        {
            var mutation = _dbContext.Mutations.FirstOrDefault(mutation =>
                mutation.Code == mutationModel.Code
            );

            return mutation;
        }
    }
}
