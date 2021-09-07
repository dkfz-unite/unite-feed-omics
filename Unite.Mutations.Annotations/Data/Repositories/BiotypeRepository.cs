using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Annotations.Data.Repositories
{
    internal class BiotypeRepository
    {
        private readonly DomainDbContext _dbContext;


        public BiotypeRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Biotype FindOrCreate(string value)
        {
            return Find(value) ?? Create(value);
        }

        public Biotype Find(string value)
        {
            var biotype = _dbContext.Biotypes.FirstOrDefault(biotype =>
                biotype.Value == value
            );

            return biotype;
        }

        public Biotype Create(string value)
        {
            var biotype = new Biotype
            {
                Value = value
            };

            _dbContext.Biotypes.Add(biotype);
            _dbContext.SaveChanges();

            return biotype;
        }
    }
}
