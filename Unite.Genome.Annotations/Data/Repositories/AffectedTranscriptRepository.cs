using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Genome.Mutations.Enums;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;

namespace Unite.Genome.Annotations.Data.Repositories
{
    public class AffectedTranscriptRepository
    {
        private readonly DomainDbContext _dbContext;
        private readonly MutationRepository _mutationRepository;
        private readonly TranscriptRepository _transcriptRepository;


        public AffectedTranscriptRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _mutationRepository = new MutationRepository(dbContext);
            _transcriptRepository = new TranscriptRepository(dbContext);
        }


        public AffectedTranscript FindOrCreate(AffectedTranscriptModel model)
        {
            return Find(model) ?? Create(model);
        }

        public AffectedTranscript Find(AffectedTranscriptModel model)
        {
            var mutation = _mutationRepository.Find(model.Mutation);

            if (mutation == null)
            {
                return null;
            }

            var transcript = _transcriptRepository.Find(model.Transcript);

            if (transcript == null)
            {
                return null;
            }

            var entity = _dbContext.Set<AffectedTranscript>()
                .FirstOrDefault(entity =>
                    entity.MutationId == mutation.Id &&
                    entity.TranscriptId == transcript.Id
                );

            return entity;
        }

        public AffectedTranscript Create(AffectedTranscriptModel model)
        {
            var entity = Convert(model);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public IEnumerable<AffectedTranscript> CreateMissing(IEnumerable<AffectedTranscriptModel> models)
        {
            var entitiesToAdd = new List<AffectedTranscript>();

            foreach (var model in models)
            {
                var entity = Find(model);

                if (entity == null)
                {
                    entity = Convert(model);

                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                _dbContext.AddRange(entitiesToAdd);
                _dbContext.SaveChanges();
            }

            return entitiesToAdd;
        }


        private AffectedTranscript Convert(AffectedTranscriptModel model)
        {
            var entity = new AffectedTranscript
            {
                CDNAStart = model.CDNAStart,
                CDNAEnd = model.CDNAEnd,
                CDSStart = model.CDSStart,
                CDSEnd = model.CDSEnd,
                ProteinStart = model.ProteinStart,
                ProteinEnd = model.ProteinEnd,
                AminoAcidChange = model.AminoAcidChange,
                CodonChange = model.CodonChange
            };

            entity.Consequences = model.Consequences.Select(Convert).ToArray();
            entity.Mutation = _mutationRepository.Find(model.Mutation);
            entity.Transcript = _transcriptRepository.FindOrCreate(model.Transcript);

            return entity;
        }

        private AffectedTranscriptConsequence Convert(ConsequenceType consequenceType)
        {
            return new AffectedTranscriptConsequence
            {
                ConsequenceId = consequenceType
            };
        }
    }
}
