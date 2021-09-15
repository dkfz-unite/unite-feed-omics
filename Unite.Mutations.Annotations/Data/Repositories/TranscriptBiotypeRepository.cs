using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Annotations.Data.Repositories
{
    internal class TranscriptBiotypeRepository
    {
        private readonly DomainDbContext _dbContext;


        public TranscriptBiotypeRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public TranscriptBiotype FindOrCreate(string value)
        {
            return Find(value) ?? Create(value);
        }

        public TranscriptBiotype Find(string value)
        {
            var biotype = _dbContext.TranscriptBiotypes.FirstOrDefault(biotype =>
                biotype.Value == value
            );

            return biotype;
        }

        public TranscriptBiotype Create(string value)
        {
            var biotype = new TranscriptBiotype
            {
                Value = value
            };

            _dbContext.TranscriptBiotypes.Add(biotype);
            _dbContext.SaveChanges();

            return biotype;
        }
    }
}
