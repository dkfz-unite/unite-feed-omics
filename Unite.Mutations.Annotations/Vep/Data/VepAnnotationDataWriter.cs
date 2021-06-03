using System.Collections.Generic;
using System.Linq;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Vep.Data.Models;
using Unite.Mutations.Annotations.Vep.Data.Models.Audit;
using Unite.Mutations.Annotations.Vep.Data.Repositories;

namespace Unite.Mutations.Annotations.Vep.Data
{
    internal class VepAnnotationDataWriter : DataWriter<AnnotationsModel, AnnotationsUploadAudit>
    {
        private readonly MutationRepository _mutationRepository;
        private readonly GeneRepository _geneRepository;
        private readonly TranscriptRepository _transcriptRepository;
        private readonly AffectedTranscriptRepository _affectedTranscriptRepository;


        public VepAnnotationDataWriter(UniteDbContext dbContext) : base(dbContext)
        {
            _mutationRepository = new MutationRepository(dbContext);
            _geneRepository = new GeneRepository(dbContext);
            _transcriptRepository = new TranscriptRepository(dbContext);
            _affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext);
        }


        protected override void ProcessModel(AnnotationsModel model, ref AnnotationsUploadAudit audit)
        {
            var mutation = _mutationRepository.Find(model.Mutation);

            audit.Mutations.Add(mutation.Id);


            if (model.AffectedTranscripts != null)
            {
                var geneModels = model.AffectedTranscripts
                    .Select(affectedTranscriptModel => affectedTranscriptModel.Gene)
                    .GroupBy(geneModel => geneModel.EnsemblId)
                    .Select(group => group.First())
                    .ToArray();

                var genes = _geneRepository.CreateMissing(geneModels);
                var genesCreated = genes.Count();


                var transcriptModels = model.AffectedTranscripts
                    .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
                    .GroupBy(transcriptModel => transcriptModel.EnsemblId)
                    .Select(group => group.First())
                    .ToArray();

                var transcripts = _transcriptRepository.CreateMissing(transcriptModels);
                var transcriptsCreated = transcripts.Count();


                var affectedTranscriptModels = model.AffectedTranscripts
                    .ToArray();

                var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels);
                var affectedTranscriptsCreated = affectedTranscripts.Count();


                audit.GenesCreated += genesCreated;
                audit.TranscriptsCreated += transcriptsCreated;
                audit.AffectedTranscriptsCreated += affectedTranscriptsCreated;
            }
        }

        protected override void ProcessModels(IEnumerable<AnnotationsModel> annotationsModels, ref AnnotationsUploadAudit audit)
        {
            foreach (var annotationsModel in annotationsModels)
            {
                var mutation = _mutationRepository.Find(annotationsModel.Mutation);

                audit.Mutations.Add(mutation.Id);
            }


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
        }
    }
}
