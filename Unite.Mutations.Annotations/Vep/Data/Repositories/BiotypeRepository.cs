using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Annotations.Vep.Data.Repositories
{
    internal class BiotypeRepository
    {
        private readonly UniteDbContext _dbContext;


        public BiotypeRepository(UniteDbContext dbContext)
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
            var biotype = new Biotype();

            biotype.Value = value;

            _dbContext.Biotypes.Add(biotype);
            _dbContext.SaveChanges();

            return biotype;
        }
    }
}
