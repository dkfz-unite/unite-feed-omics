using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;
using Unite.Genome.Annotations.Data.Models.Audit;
using Unite.Genome.Annotations.Data.Repositories;

namespace Unite.Genome.Annotations.Data;

public class AnnotationsDataWriter : DataWriter<AnnotationsModel, AnnotationsUploadAudit>
{
    private readonly MutationRepository _mutationRepository;
    private readonly GeneRepository _geneRepository;
    private readonly ProteinRepository _proteinRepository;
    private readonly TranscriptRepository _transcriptRepository;
    private readonly AffectedTranscriptRepository _affectedTranscriptRepository;


    public AnnotationsDataWriter(DomainDbContext dbContext) : base(dbContext)
    {
        _mutationRepository = new MutationRepository(dbContext);
        _geneRepository = new GeneRepository(dbContext);
        _proteinRepository = new ProteinRepository(dbContext);
        _transcriptRepository = new TranscriptRepository(dbContext);
        _affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext);
    }


    protected override void ProcessModel(AnnotationsModel annotationsModel, ref AnnotationsUploadAudit audit)
    {
        var mutation = _mutationRepository.Find(annotationsModel.Mutation);

        audit.Mutations.Add(mutation.Id);


        if (annotationsModel.AffectedTranscripts != null)
        {
            var geneModels = annotationsModel.AffectedTranscripts
                .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript.Gene)
                .DistinctBy(geneModel => geneModel.EnsemblId);

            var genes = _geneRepository.CreateMissing(geneModels);
            var genesCreated = genes.Count();


            var proteinModels = annotationsModel.AffectedTranscripts
                .Where(affectedTranscriptModel => affectedTranscriptModel.Transcript.Protein != null)
                .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript.Protein)
                .DistinctBy(proteinModel => proteinModel.EnsemblId);

            var proteins = _proteinRepository.CreateMissing(proteinModels);
            var proteinsCreated = proteins.Count();


            var transcriptModels = annotationsModel.AffectedTranscripts
                .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
                .DistinctBy(transcriptModel => transcriptModel.EnsemblId);

            var transcripts = _transcriptRepository.CreateMissing(transcriptModels);
            var transcriptsCreated = transcripts.Count();


            var affectedTranscriptModels = annotationsModel.AffectedTranscripts;

            var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels);
            var affectedTranscriptsCreated = affectedTranscripts.Count();


            audit.GenesCreated += genesCreated;
            audit.ProteinsCreated += proteinsCreated;
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
            .Where(annotationsModel => annotationsModel.AffectedTranscripts != null)
            .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript.Gene)
            .DistinctBy(geneModel => geneModel.EnsemblId);

        var genes = _geneRepository.CreateMissing(geneModels);
        var genesCreated = genes.Count();


        var proteinModels = annotationsModels
            .Where(annotationsModel => annotationsModel.AffectedTranscripts != null)
            .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts)
            .Where(affectedTranscriptModel => affectedTranscriptModel.Transcript.Protein != null)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript.Protein)
            .DistinctBy(proteinModel => proteinModel.EnsemblId);

        var proteins = _proteinRepository.CreateMissing(proteinModels);
        var proteinsCreated = proteins.Count();


        var transcriptModels = annotationsModels
            .Where(annotationsModel => annotationsModel.AffectedTranscripts != null)
            .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
            .DistinctBy(transcriptModel => transcriptModel.EnsemblId);

        var transcripts = _transcriptRepository.CreateMissing(transcriptModels);
        var transcriptsCreated = transcripts.Count();


        var affectedTranscriptModels = annotationsModels
            .Where(annotationsModel => annotationsModel.AffectedTranscripts != null)
            .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts);

        var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels);
        var affectedTranscriptsCreated = affectedTranscripts.Count();


        audit.GenesCreated += genesCreated;
        audit.ProteinsCreated += proteinsCreated;
        audit.TranscriptsCreated += transcriptsCreated;
        audit.AffectedTranscriptsCreated += affectedTranscriptsCreated;
    }
}
