using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Vep.Data.Models;

namespace Unite.Mutations.Annotations.Vep.Data.Repositories
{
    internal class AffectedTranscriptRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly MutationRepository _mutationRepository;
        private readonly GeneRepository _geneRepository;
        private readonly TranscriptRepository _transcriptRepository;


        public AffectedTranscriptRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _mutationRepository = new MutationRepository(dbContext);
            _geneRepository = new GeneRepository(dbContext);
            _transcriptRepository = new TranscriptRepository(dbContext);
        }

        public AffectedTranscript FindOrCreate(AffectedTranscriptModel affectedTranscriptModel)
        {
            return Find(affectedTranscriptModel) ?? Create(affectedTranscriptModel);
        }

        public AffectedTranscript Find(AffectedTranscriptModel affectedTranscriptModel)
        {
            var mutation = _mutationRepository.Find(affectedTranscriptModel.Mutation);

            if(mutation == null)
            {
                return null;
            }

            var gene = _geneRepository.Find(affectedTranscriptModel.Gene);

            if(gene == null)
            {
                return null;
            }

            var transcript = _transcriptRepository.Find(affectedTranscriptModel.Transcript);

            if(transcript == null)
            {
                return null;
            }

            return Find(mutation.Id, gene.Id, transcript.Id);
        }

        public AffectedTranscript Create(AffectedTranscriptModel affectedTranscriptModel)
        {
            var mutation = _mutationRepository.Find(affectedTranscriptModel.Mutation);
            var gene = _geneRepository.FindOrCreate(affectedTranscriptModel.Gene);
            var transcript = _transcriptRepository.FindOrCreate(affectedTranscriptModel.Transcript);

            return Create(mutation.Id, gene.Id, transcript.Id, affectedTranscriptModel);
        }

        public IEnumerable<AffectedTranscript> CreateMissing(IEnumerable<AffectedTranscriptModel> affectedTranscriptModels)
        {
            var affectedTranscriptsToAdd = new List<AffectedTranscript>();

            foreach(var affectedTranscriptModel in affectedTranscriptModels)
            {
                var mutation = _mutationRepository.Find(affectedTranscriptModel.Mutation);
                var gene = _geneRepository.FindOrCreate(affectedTranscriptModel.Gene);
                var transcript = _transcriptRepository.FindOrCreate(affectedTranscriptModel.Transcript);

                var affectedTranscript = Find(mutation.Id, gene.Id, transcript.Id);

                if(affectedTranscript == null)
                {
                    affectedTranscript = Convert(affectedTranscriptModel);

                    affectedTranscript.MutationId = mutation.Id;
                    affectedTranscript.GeneId = gene.Id;
                    affectedTranscript.TranscriptId = transcript.Id;

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


        private AffectedTranscript Find(long mutationId, int geneId, int transcriptId)
        {
            var affectedTranscript = _dbContext.AffectedTranscripts.FirstOrDefault(affectedTranscript =>
                affectedTranscript.MutationId == mutationId &&
                affectedTranscript.GeneId == geneId &&
                affectedTranscript.TranscriptId == transcriptId
            );

            return affectedTranscript;
        }

        private AffectedTranscript Create(long mutationId, int geneId, int transcriptId, AffectedTranscriptModel affectedTranscriptModel)
        {
            var affectedTranscript = Convert(affectedTranscriptModel);

            affectedTranscript.MutationId = mutationId;
            affectedTranscript.GeneId = geneId;
            affectedTranscript.TranscriptId = transcriptId;

            _dbContext.AffectedTranscripts.Add(affectedTranscript);
            _dbContext.SaveChanges();

            return affectedTranscript;

        }

        private AffectedTranscript Convert(AffectedTranscriptModel affectedTranscriptModel)
        {
            var affectedTranscript = new AffectedTranscript();

            affectedTranscript.CDNAStart = affectedTranscriptModel.CDNAStart;
            affectedTranscript.CDNAEnd = affectedTranscriptModel.CDNAEnd;
            affectedTranscript.CDSStart = affectedTranscriptModel.CDSStart;
            affectedTranscript.CDSEnd = affectedTranscriptModel.CDSEnd;
            affectedTranscript.ProteinStart = affectedTranscriptModel.ProteinStart;
            affectedTranscript.ProteinEnd = affectedTranscriptModel.ProteinEnd;
            affectedTranscript.AminoAcidChange = affectedTranscriptModel.AminoAcidChange;
            affectedTranscript.CodonChange = affectedTranscriptModel.CodonChange;

            affectedTranscript.Consequences = affectedTranscriptModel.Consequences.Select(consequenceModel =>
            {
                var affectedTrancriptConsequence = new AffectedTranscriptConsequence();

                affectedTrancriptConsequence.ConsequenceId = consequenceModel.Type;

                return affectedTrancriptConsequence;

            }).ToArray();

            return affectedTranscript;
        }
    }
}
