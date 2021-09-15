using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Data.Models;

namespace Unite.Mutations.Annotations.Data.Repositories
{
    public class TranscriptRepository
    {
        private readonly DomainDbContext _dbContext;
        private readonly TranscriptBiotypeRepository _biotypeRepository;
        private readonly GeneRepository _geneRepository;
        private readonly ProteinRepository _proteinRepository;


        public TranscriptRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _biotypeRepository = new TranscriptBiotypeRepository(dbContext);
            _geneRepository = new GeneRepository(dbContext);
            _proteinRepository = new ProteinRepository(dbContext);
        }


        public Transcript FindOrCreate(TranscriptModel transcriptModel)
        {
            return Find(transcriptModel) ?? Create(transcriptModel);
        }

        public Transcript Find(TranscriptModel transcriptModel)
        {
            var transcript = _dbContext.Transcripts.FirstOrDefault(transcript =>
                transcript.Info.EnsemblId == transcriptModel.EnsemblId
            );

            return transcript;
        }

        public Transcript Create(TranscriptModel transcriptModel)
        {
            var transcript = Convert(transcriptModel);

            _dbContext.Transcripts.Add(transcript);
            _dbContext.SaveChanges();

            return transcript;
        }

        public IEnumerable<Transcript> CreateMissing(IEnumerable<TranscriptModel> transcriptModels)
        {
            var transcriptsToAdd = new List<Transcript>();

            foreach (var transcriptModel in transcriptModels)
            {
                var transcript = Find(transcriptModel);

                if (transcript == null)
                {
                    transcript = Convert(transcriptModel);

                    transcriptsToAdd.Add(transcript);
                }
            }

            if (transcriptsToAdd.Any())
            {
                _dbContext.Transcripts.AddRange(transcriptsToAdd);
                _dbContext.SaveChanges();
            }

            return transcriptsToAdd;
        }


        private Transcript Convert(TranscriptModel transcriptModel)
        {
            var transcript = new Transcript
            {
                Symbol = transcriptModel.Symbol,
                ChromosomeId = transcriptModel.Chromosome,
                Start = transcriptModel.Start,
                End = transcriptModel.End,
                Strand = transcriptModel.Strand,

                Info = new TranscriptInfo
                {
                    EnsemblId = transcriptModel.EnsemblId
                }
            };

            if (transcriptModel.Biotype != null)
            {
                transcript.BiotypeId = _biotypeRepository.FindOrCreate(transcriptModel.Biotype).Id;
            }

            if (transcriptModel.Gene != null)
            {
                transcript.GeneId = _geneRepository.FindOrCreate(transcriptModel.Gene).Id;
            }

            if (transcriptModel.Protein != null)
            {
                transcript.ProteinId = _proteinRepository.FindOrCreate(transcriptModel.Protein).Id;
            }

            return transcript;
        }
    }
}
