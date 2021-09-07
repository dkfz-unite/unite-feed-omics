using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Mutations.Enums;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Data.Models;

namespace Unite.Mutations.Annotations.Data.Repositories
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


        public AffectedTranscript FindOrCreate(AffectedTranscriptModel affectedTranscriptModel)
        {
            return Find(affectedTranscriptModel) ?? Create(affectedTranscriptModel);
        }

        public AffectedTranscript Find(AffectedTranscriptModel affectedTranscriptModel)
        {
            var mutation = _mutationRepository.Find(affectedTranscriptModel.Mutation);

            if (mutation == null)
            {
                return null;
            }

            var transcript = _transcriptRepository.Find(affectedTranscriptModel.Transcript);

            if (transcript == null)
            {
                return null;
            }

            var affectedTranscript = _dbContext.AffectedTranscripts.FirstOrDefault(affectedTranscript =>
                affectedTranscript.MutationId == mutation.Id &&
                affectedTranscript.TranscriptId == transcript.Id
            );

            return affectedTranscript;
        }

        public AffectedTranscript Create(AffectedTranscriptModel affectedTranscriptModel)
        {
            var affectedTranscript = Convert(affectedTranscriptModel);

            _dbContext.AffectedTranscripts.Add(affectedTranscript);
            _dbContext.SaveChanges();

            return affectedTranscript;
        }

        public IEnumerable<AffectedTranscript> CreateMissing(IEnumerable<AffectedTranscriptModel> affectedTranscriptModels)
        {
            var affectedTranscriptsToAdd = new List<AffectedTranscript>();

            foreach (var affectedTranscriptModel in affectedTranscriptModels)
            {
                var affectedTranscript = Find(affectedTranscriptModel);

                if (affectedTranscript == null)
                {
                    affectedTranscript = Convert(affectedTranscriptModel);

                    affectedTranscriptsToAdd.Add(affectedTranscript);
                }
            }

            if (affectedTranscriptsToAdd.Any())
            {
                _dbContext.AffectedTranscripts.AddRange(affectedTranscriptsToAdd);
                _dbContext.SaveChanges();
            }

            return affectedTranscriptsToAdd;
        }


        private AffectedTranscript Convert(AffectedTranscriptModel affectedTranscriptModel)
        {
            var affectedTranscript = new AffectedTranscript
            {
                CDNAStart = affectedTranscriptModel.CDNAStart,
                CDNAEnd = affectedTranscriptModel.CDNAEnd,
                CDSStart = affectedTranscriptModel.CDSStart,
                CDSEnd = affectedTranscriptModel.CDSEnd,
                ProteinStart = affectedTranscriptModel.ProteinStart,
                ProteinEnd = affectedTranscriptModel.ProteinEnd,
                AminoAcidChange = affectedTranscriptModel.AminoAcidChange,
                CodonChange = affectedTranscriptModel.CodonChange
            };

            affectedTranscript.Consequences = affectedTranscriptModel.Consequences.Select(Convert).ToArray();
            affectedTranscript.Mutation = _mutationRepository.Find(affectedTranscriptModel.Mutation);
            affectedTranscript.Transcript = _transcriptRepository.FindOrCreate(affectedTranscriptModel.Transcript);

            return affectedTranscript;
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
