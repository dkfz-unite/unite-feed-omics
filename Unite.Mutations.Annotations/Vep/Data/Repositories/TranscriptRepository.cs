using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Vep.Data.Models;

namespace Unite.Mutations.Annotations.Vep.Data.Repositories
{
    internal class TranscriptRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly BiotypeRepository _biotypeRepository;


        public TranscriptRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _biotypeRepository = new BiotypeRepository(dbContext);
        }

        public Transcript FindOrCreate(TranscriptModel transcriptModel)
        {
            return Find(transcriptModel) ?? Create(transcriptModel);
        }

        public Transcript Find(TranscriptModel transcriptModel)
        {
            return Find(transcriptModel.EnsemblId);
        }

        public Transcript Create(TranscriptModel transcriptModel)
        {
            var transcript = Convert(transcriptModel);

            if (!string.IsNullOrWhiteSpace(transcriptModel.Biotype))
            {
                transcript.Biotype = _biotypeRepository.FindOrCreate(transcriptModel.Biotype);
            }

            _dbContext.Transcripts.Add(transcript);
            _dbContext.SaveChanges();

            return transcript;
        }

        public IEnumerable<Transcript> CreateMissing(IEnumerable<TranscriptModel> transcriptModels)
        {
            var transcriptsToAdd = new List<Transcript>();

            foreach(var transcriptModel in transcriptModels)
            {
                var transcript = Find(transcriptModel.EnsemblId);

                if(transcript == null)
                {
                    transcript = Convert(transcriptModel);

                    if (!string.IsNullOrWhiteSpace(transcriptModel.Biotype))
                    {
                        transcript.Biotype = _biotypeRepository.FindOrCreate(transcriptModel.Biotype);
                    }

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


        private Transcript Find(string ensemblId)
        {
            var transcript = _dbContext.Transcripts.FirstOrDefault(transcript =>
                transcript.Info.EnsemblId == ensemblId
            );

            return transcript;
        }

        private Transcript Convert(TranscriptModel transcriptModel)
        {
            var transcript = new Transcript();

            transcript.Strand = transcriptModel.Strand;

            transcript.Info = new TranscriptInfo
            {
                EnsemblId = transcriptModel.EnsemblId
            };

            return transcript;
        }
    }
}
