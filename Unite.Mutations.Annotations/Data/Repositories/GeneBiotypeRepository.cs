using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Annotations.Data.Repositories
{
    internal class GeneBiotypeRepository
    {
        private readonly DomainDbContext _dbContext;


        public GeneBiotypeRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public GeneBiotype FindOrCreate(string value)
        {
            return Find(value) ?? Create(value);
        }

        public GeneBiotype Find(string value)
        {
            var biotype = _dbContext.GeneBiotypes.FirstOrDefault(biotype =>
                biotype.Value == value
            );

            return biotype;
        }

        public GeneBiotype Create(string value)
        {
            var biotype = new GeneBiotype
            {
                Value = value
            };

            _dbContext.GeneBiotypes.Add(biotype);
            _dbContext.SaveChanges();

            return biotype;
        }
    }
}
