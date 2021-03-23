using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class TranscriptRepository : Repository<Transcript>
    {
        private readonly Repository<Biotype> _biotypeRepository;

        public TranscriptRepository(DbContext dbContext) : base(dbContext)
        {
            _biotypeRepository = new BiotypeRepository(dbContext);
        }

        protected override void Map(in Transcript source, ref Transcript target)
        {
            target.Symbol = source.Symbol;
            target.ChromosomeId = source.ChromosomeId;
            target.Start = source.Start;
            target.End = source.End;
            target.Strand = source.Strand;

            if(source.Biotype != null)
            {
                target.Biotype = GetOrCreate(source.Biotype);
            }

            if (source.Info != null)
            {
                if (target.Info == null)
                {
                    target.Info = new TranscriptInfo();
                }

                Map(source.Info, target.Info);
            }
        }

        private void Map(in TranscriptInfo source, TranscriptInfo target)
        {
            target.EnsemblId = source.EnsemblId;
        }

        private Biotype GetOrCreate(in Biotype model)
        {
            var value = model.Value;

            var entity = _biotypeRepository.Entities
                .FirstOrDefault(entity =>
                    entity.Value == value
            );

            if (entity == null)
            {
                entity = _biotypeRepository.Add(model);
            }

            return entity;
        }
    }
}
