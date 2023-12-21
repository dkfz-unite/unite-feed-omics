using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Models.Variants;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Variants;

namespace Unite.Genome.Feed.Data.Writers.Variants;

public abstract class ConsequencesDataWriter<TAffectedTranscriptEntity, TVariantEntity, TVariantModel> : DataWriter<ConsequencesDataModel, ConsequencesDataUploadAudit>
    where TAffectedTranscriptEntity : VariantAffectedFeature<TVariantEntity, Transcript>
    where TVariantEntity : Variant
    where TVariantModel : VariantModel
{
    protected DomainDbContext _dbContext;
    protected GeneRepository _geneRepository;
    protected ProteinRepository _proteinRepository;
    protected TranscriptRepository _transcriptRepository;
    protected VariantRepository<TVariantEntity, TVariantModel> _variantRepository;
    protected AffectedTranscriptRepository<TAffectedTranscriptEntity, TVariantEntity, TVariantModel> _affectedTranscriptRepository;

    protected ConsequencesDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    protected override void ProcessModel(ConsequencesDataModel consequencesDataModel, ref ConsequencesDataUploadAudit audit)
    {
        var variant = _variantRepository.Find(consequencesDataModel.VariantId);
        var variants = new [] { variant };
        audit.Variants.Add(variant.Id);

        var geneModels = GetUniqueGeneModels(consequencesDataModel);
        var genes = _geneRepository.CreateMissing(geneModels);
        audit.GenesCreated += genes.Count();

        var transcriptModels = GetUniqueTranscriptModels(consequencesDataModel);
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels, genesCache: genes);
        audit.TranscriptsCreated += transcripts.Count();

        var proteinModels = GetUniqueProteinModels(consequencesDataModel);
        var proteins = _proteinRepository.CreateMissing(proteinModels, transcriptsCache: transcripts, genesCache: genes);
        audit.ProteinsCreated += proteins.Count();

        if (consequencesDataModel.AffectedTranscripts != null)
        {
            var variantIds = variants.Select(variant => variant.Id).ToArray();
            _affectedTranscriptRepository.RemoveAll(variantIds);

            var affectedTranscriptModels = consequencesDataModel.AffectedTranscripts;
            var affectedTranscripts = _affectedTranscriptRepository.CreateAll(affectedTranscriptModels, variants, transcripts);
            audit.AffectedTranscriptsCreated += affectedTranscripts.Count();
        }
    }

    protected override void ProcessModels(IEnumerable<ConsequencesDataModel> consequencesDataModels, ref ConsequencesDataUploadAudit audit)
    {
        var variantIds = consequencesDataModels.Select(model => model.VariantId);
        var variants = _variantRepository.Find(variantIds);
        audit.Variants.AddRange(variants.Select(variant => variant.Id));

        var geneModels = GetUniqueGeneModels(consequencesDataModels.ToArray());
        var genes = _geneRepository.CreateMissing(geneModels);
        audit.GenesCreated += genes.Count();

        var transcriptModels = GetUniqueTranscriptModels(consequencesDataModels.ToArray());
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels, genesCache: genes);
        audit.TranscriptsCreated += transcripts.Count();

        var proteinModels = GetUniqueProteinModels(consequencesDataModels.ToArray());
        var proteins = _proteinRepository.CreateMissing(proteinModels, transcriptsCache: transcripts, genesCache: genes);
        audit.ProteinsCreated += proteins.Count();

        if (consequencesDataModels.Any(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null))
        {
            _affectedTranscriptRepository.RemoveAll(variantIds);

            var affectedTranscriptModels = consequencesDataModels
                .Where(annotationsModel => annotationsModel.AffectedTranscripts != null)
                .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts)
                .ToArray();

            var affectedTranscripts = _affectedTranscriptRepository.CreateAll(affectedTranscriptModels, variants, transcripts);

            audit.AffectedTranscriptsCreated += affectedTranscripts.Count();
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
