using System.Collections.Generic;
using System.Linq;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Vep.Data.Models;
using Unite.Mutations.Annotations.Vep.Data.Models.Audit;
using Unite.Mutations.Annotations.Vep.Data.Repositories;

namespace Unite.Mutations.Annotations.Vep.Data
{
    internal class VepAnnotationDataWriter
    {
        private readonly UniteDbContext _dbContext;
        private readonly GeneRepository _geneRepository;
        private readonly TranscriptRepository _transcriptRepository;
        private readonly AffectedTranscriptRepository _affectedTranscriptRepository;


        public VepAnnotationDataWriter(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _geneRepository = new GeneRepository(dbContext);
            _transcriptRepository = new TranscriptRepository(dbContext);
            _affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext);
        }


        public void SaveData(AnnotationsModel model, out AnnotationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new AnnotationsUploadAudit();

                ProcessModel(model, ref audit);

                transaction.Commit();
            }
            catch
            {
                audit = null;

                transaction.Rollback();

                throw;
            }
        }

        public void SaveData(IEnumerable<AnnotationsModel> models, out AnnotationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new AnnotationsUploadAudit();

                ProcessModels(models, ref audit);

                transaction.Commit();
            }
            catch
            {
                audit = null;

                transaction.Rollback();

                throw;
            }
        }


        private void ProcessModel(AnnotationsModel annotationsModel, ref AnnotationsUploadAudit audit)
        {
            if (annotationsModel.AffectedTranscripts != null)
            {
                var geneModels = annotationsModel.AffectedTranscripts
                    .Select(affectedTranscriptModel => affectedTranscriptModel.Gene)
                    .GroupBy(geneModel => geneModel.EnsemblId)
                    .Select(group => group.First())
                    .ToArray();

                var genes = _geneRepository.CreateMissing(geneModels);
                var genesCreated = genes.Count();


                var transcriptModels = annotationsModel.AffectedTranscripts
                    .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
                    .GroupBy(transcriptModel => transcriptModel.EnsemblId)
                    .Select(group => group.First())
                    .ToArray();

                var transcripts = _transcriptRepository.CreateMissing(transcriptModels);
                var transcriptsCreated = transcripts.Count();


                var affectedTranscriptModels = annotationsModel.AffectedTranscripts
                    .ToArray();

                var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels);
                var affectedTranscriptsCreated = affectedTranscripts.Count();


                audit.GenesCreated += genesCreated;
                audit.TranscriptsCreated += transcriptsCreated;
                audit.AffectedTranscriptsCreated += affectedTranscriptsCreated;

                if (transcriptsCreated > 0)
                {
                    foreach (var affectedTranscript in affectedTranscripts)
                    {
                        audit.Mutations.Add(affectedTranscript.MutationId);
                    }
                }
            }
        }

        private void ProcessModels(IEnumerable<AnnotationsModel> annotationsModels, ref AnnotationsUploadAudit audit)
        {
            var geneModels = annotationsModels
                .Where(annotationmodel => annotationmodel.AffectedTranscripts != null)
                .SelectMany(annotationModel => annotationModel.AffectedTranscripts)
                .Select(affectedTranscriptModel => affectedTranscriptModel.Gene)
                .GroupBy(geneModel => geneModel.EnsemblId)
                .Select(group => group.First())
                .ToArray();

            var genes = _geneRepository.CreateMissing(geneModels);
            var genesCreated = genes.Count();


            var transcriptModels = annotationsModels
                .Where(annotationmodel => annotationmodel.AffectedTranscripts != null)
                .SelectMany(annotationModel => annotationModel.AffectedTranscripts)
                .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
                .GroupBy(transcriptModel => transcriptModel.EnsemblId)
                .Select(group => group.First())
                .ToArray();

            var transcripts = _transcriptRepository.CreateMissing(transcriptModels);
            var transcriptsCreated = transcripts.Count();


            var affectedTranscriptModels = annotationsModels
                .Where(annotationmodel => annotationmodel.AffectedTranscripts != null)
                .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts)
                .ToArray();

            var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels);
            var affectedTranscriptsCreated = affectedTranscripts.Count();


            audit.GenesCreated += genesCreated;
            audit.TranscriptsCreated += transcriptsCreated;
            audit.AffectedTranscriptsCreated += affectedTranscriptsCreated;

            if (transcriptsCreated > 0)
            {
                foreach(var affectedTranscript in affectedTranscripts)
                {
                    audit.Mutations.Add(affectedTranscript.MutationId);
                }
            }
        }
    }
}
