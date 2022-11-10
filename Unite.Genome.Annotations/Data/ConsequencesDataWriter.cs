using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;
using Unite.Genome.Annotations.Data.Models.Variants;
using Unite.Genome.Annotations.Data.Repositories;

namespace Unite.Genome.Annotations.Data;

internal class ConsequencesDataWriter<TVariant, TAffectedTranscript> : DataWriter<ConsequencesDataModel, ConsequencesDataUploadAudit>
    where TVariant : Variant
    where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
{
    protected readonly GeneRepository _geneRepository;
    protected readonly ProteinRepository _proteinRepository;
    protected readonly TranscriptRepository _transcriptRepository;
    protected readonly VariantRepository<TVariant> _variantRepository;
    protected readonly AffectedTranscriptRepository<TVariant, TAffectedTranscript> _affectedTranscriptRepository;


    public ConsequencesDataWriter(
        DomainDbContext dbContext,
        VariantRepository<TVariant> variantRepository,
        AffectedTranscriptRepository<TVariant, TAffectedTranscript> affectedTranscriptRepository)
        : base(dbContext)
    {
        _geneRepository = new GeneRepository(dbContext);
        _proteinRepository = new ProteinRepository(dbContext);
        _transcriptRepository = new TranscriptRepository(dbContext);
        _variantRepository = variantRepository;
        _affectedTranscriptRepository = affectedTranscriptRepository;
    }


    protected override void ProcessModel(ConsequencesDataModel consequencesDataModel, ref ConsequencesDataUploadAudit audit)
    {
        var variant = _variantRepository.Find(consequencesDataModel.Variant);

        audit.Variants.Add(variant.Id);


        var geneModels = GetUniqueGeneModels(consequencesDataModel);
        var genes = _geneRepository.CreateMissing(geneModels);
        var genesCreated = genes.Count();

        var proteinModels = GetUniqueProteinModels(consequencesDataModel);
        var proteins = _proteinRepository.CreateMissing(proteinModels);
        var proteinsCreated = proteins.Count();

        var transcriptModels = GetUniqueTranscriptModels(consequencesDataModel);
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels, genes, proteins);
        var transcriptsCreated = transcripts.Count();

        audit.GenesCreated += genesCreated;
        audit.ProteinsCreated += proteinsCreated;
        audit.TranscriptsCreated += transcriptsCreated;


        if (consequencesDataModel.AffectedTranscripts != null)
        {
            var affectedTranscriptModels = consequencesDataModel.AffectedTranscripts;
            var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels, null, transcripts);
            var affectedTranscriptsCreated = affectedTranscripts.Count();

            audit.AffectedTranscriptsCreated += affectedTranscriptsCreated;
        }
    }

    protected override void ProcessModels(IEnumerable<ConsequencesDataModel> consequencesDataModels, ref ConsequencesDataUploadAudit audit)
    {
        var variantModels = consequencesDataModels.Select(model => model.Variant).ToArray();
        var variants = _variantRepository.Find(variantModels);

        foreach (var variant in variants)
        {
            audit.Variants.Add(variant.Id);
        }


        var geneModels = GetUniqueGeneModels(consequencesDataModels.ToArray());
        var genes = _geneRepository.CreateMissing(geneModels);
        var genesCreated = genes.Count();

        var proteinModels = GetUniqueProteinModels(consequencesDataModels.ToArray());
        var proteins = _proteinRepository.CreateMissing(proteinModels);
        var proteinsCreated = proteins.Count();

        var transcriptModels = GetUniqueTranscriptModels(consequencesDataModels.ToArray());
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels);
        var transcriptsCreated = transcripts.Count();

        audit.GenesCreated += genesCreated;
        audit.ProteinsCreated += proteinsCreated;
        audit.TranscriptsCreated += transcriptsCreated;


        if (consequencesDataModels.Any(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null))
        {
            var affectedTranscriptModels = consequencesDataModels
                .Where(annotationsModel => annotationsModel.AffectedTranscripts != null)
                .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts);

            var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels, variants, transcripts);

            var affectedTranscriptsCreated = affectedTranscripts.Count();

            audit.AffectedTranscriptsCreated += affectedTranscriptsCreated;
        }
    }


    protected IEnumerable<GeneModel> GetUniqueGeneModels(params ConsequencesDataModel[] consequencesDataModels)
    {
        return consequencesDataModels
            .Where(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null)
            .SelectMany(consequencesDataModel => consequencesDataModel.AffectedTranscripts)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript.Gene)
            .DistinctBy(geneModel => geneModel.EnsemblId);
    }

    protected IEnumerable<TranscriptModel> GetUniqueTranscriptModels(params ConsequencesDataModel[] consequencesDataModels)
    {
        return consequencesDataModels
            .Where(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null)
            .SelectMany(consequencesDataModel => consequencesDataModel.AffectedTranscripts)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
            .DistinctBy(transcriptModel => transcriptModel.EnsemblId);
    }

    protected IEnumerable<ProteinModel> GetUniqueProteinModels(params ConsequencesDataModel[] consequencesDataModels)
    {
        return consequencesDataModels
            .Where(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null)
            .SelectMany(consequencesDataModel => consequencesDataModel.AffectedTranscripts)
            .Where(affectedTranscriptModel => affectedTranscriptModel.Transcript.Protein != null)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript.Protein)
            .DistinctBy(proteinModel => proteinModel.EnsemblId);
    }
}
