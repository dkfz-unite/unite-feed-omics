using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Mutations.Models;

namespace Unite.Genome.Feed.Data.Mutations.Repositories
{
    internal class MutationRepository
    {
        private readonly DomainDbContext _dbContext;


        public MutationRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Mutation FindOrCreate(MutationModel model)
        {
            return Find(model) ?? Create(model);
        }

        public Mutation Find(MutationModel model)
        {
            var entity = _dbContext.Set<Mutation>()
                .FirstOrDefault(entity =>
                    entity.Code == model.Code
                );

            return entity;
        }

        public Mutation Create(MutationModel model)
        {
            var entity = new Mutation();

            Map(model, ref entity);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public IEnumerable<Mutation> CreateMissing(IEnumerable<MutationModel> models)
        {
            var entitiesToAdd = new List<Mutation>();

            foreach (var model in models)
            {
                var entity = Find(model);

                if (entity == null)
                {
                    entity = new Mutation();

                    Map(model, ref entity);

                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                _dbContext.AddRange(entitiesToAdd);
                _dbContext.SaveChanges();
            }

            return entitiesToAdd.ToArray();
        }


        private void Map(in MutationModel model, ref Mutation entity)
        {
            entity.Code = model.Code;
            entity.ChromosomeId = model.Chromosome;
            entity.Start = model.Start;
            entity.End = model.End;
            entity.ReferenceBase = model.ReferenceBase;
            entity.AlternateBase = model.AlternateBase;
            entity.TypeId = model.Type;
        }
    }
}
