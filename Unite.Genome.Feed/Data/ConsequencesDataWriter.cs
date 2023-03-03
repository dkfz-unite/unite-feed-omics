using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Models.Variants;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Variants;

namespace Unite.Genome.Feed.Data;

public class ConsequencesDataWriter<TAffectedTranscriptEntity, TVariantEntity, TVariantModel> : DataWriter<ConsequencesDataModel, ConsequencesDataUploadAudit>
    where TAffectedTranscriptEntity : VariantAffectedFeature<TVariantEntity, Transcript>
    where TVariantEntity : Variant
    where TVariantModel : VariantModel
{
    protected readonly GeneRepository _geneRepository;
    protected readonly ProteinRepository _proteinRepository;
    protected readonly TranscriptRepository _transcriptRepository;
    protected readonly VariantRepository<TVariantEntity, TVariantModel> _variantRepository;
    protected readonly AffectedTranscriptRepository<TAffectedTranscriptEntity, TVariantEntity, TVariantModel> _affectedTranscriptRepository;


    public ConsequencesDataWriter(
        DomainDbContext dbContext,
        VariantRepository<TVariantEntity, TVariantModel> variantRepository,
        AffectedTranscriptRepository<TAffectedTranscriptEntity, TVariantEntity, TVariantModel> affectedTranscriptRepository)
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

        var transcriptModels = GetUniqueTranscriptModels(consequencesDataModel);
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels, genesCache: genes);
        var transcriptsCreated = transcripts.Count();

        var proteinModels = GetUniqueProteinModels(consequencesDataModel);
        var proteins = _proteinRepository.CreateMissing(proteinModels, transcriptsCache: transcripts, genesCache: genes);
        var proteinsCreated = proteins.Count();

        audit.GenesCreated += genesCreated;
        audit.TranscriptsCreated += transcriptsCreated;
        audit.ProteinsCreated += proteinsCreated;
        

        if (consequencesDataModel.AffectedTranscripts != null)
        {
            var affectedTranscriptModels = consequencesDataModel.AffectedTranscripts;
            var affectedTranscripts = _affectedTranscriptRepository.CreateMissing(affectedTranscriptModels, transcriptsCache: transcripts);
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

        var transcriptModels = GetUniqueTranscriptModels(consequencesDataModels.ToArray());
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels, genesCache: genes);
        var transcriptsCreated = transcripts.Count();

        var proteinModels = GetUniqueProteinModels(consequencesDataModels.ToArray());
        var proteins = _proteinRepository.CreateMissing(proteinModels, transcriptsCache: transcripts, genesCache: genes);
        var proteinsCreated = proteins.Count();

        
        audit.GenesCreated += genesCreated;
        audit.TranscriptsCreated += transcriptsCreated;
        audit.ProteinsCreated += proteinsCreated;
        

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
            .Select(affectedTranscriptModel => affectedTranscriptModel.Gene)
            .DistinctBy(geneModel => geneModel.Id);
    }

    protected IEnumerable<TranscriptModel> GetUniqueTranscriptModels(params ConsequencesDataModel[] consequencesDataModels)
    {
        return consequencesDataModels
            .Where(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null)
            .SelectMany(consequencesDataModel => consequencesDataModel.AffectedTranscripts)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
            .DistinctBy(transcriptModel => transcriptModel.Id);
    }

    protected IEnumerable<ProteinModel> GetUniqueProteinModels(params ConsequencesDataModel[] consequencesDataModels)
    {
        return consequencesDataModels
            .Where(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null)
            .SelectMany(consequencesDataModel => consequencesDataModel.AffectedTranscripts)
            .Where(affectedTranscriptModel => affectedTranscriptModel.Protein != null)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Protein)
            .DistinctBy(proteinModel => proteinModel.Id);
    }
}
