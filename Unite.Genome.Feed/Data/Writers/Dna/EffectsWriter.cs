using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Models.Dna;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Dna;

namespace Unite.Genome.Feed.Data.Writers.Dna;

public abstract class EffectsWriter<TAffectedTranscriptEntity, TVariantEntity, TVariantModel> : DataWriter<EffectsDataModel, EffectsWriteAudit>
    where TAffectedTranscriptEntity : VariantAffectedFeature<TVariantEntity, Transcript>
    where TVariantEntity : Variant
    where TVariantModel : VariantModel
{
    protected GeneRepository _geneRepository;
    protected ProteinRepository _proteinRepository;
    protected TranscriptRepository _transcriptRepository;
    protected VariantRepository<TVariantEntity, TVariantModel> _variantRepository;
    protected AffectedTranscriptRepository<TAffectedTranscriptEntity, TVariantEntity, TVariantModel> _affectedTranscriptRepository;


    protected EffectsWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    protected override void ProcessModel(EffectsDataModel effectsDataModel, ref EffectsWriteAudit audit)
    {
        var variant = _variantRepository.Find(effectsDataModel.VariantId);
        var variants = new [] { variant };
        audit.Variants.Add(variant.Id);

        var geneModels = GetUniqueGeneModels(effectsDataModel);
        var genes = _geneRepository.CreateMissing(geneModels);
        audit.GenesCreated += genes.Count();

        var transcriptModels = GetUniqueTranscriptModels(effectsDataModel);
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels, genesCache: genes);
        audit.TranscriptsCreated += transcripts.Count();

        var proteinModels = GetUniqueProteinModels(effectsDataModel);
        var proteins = _proteinRepository.CreateMissing(proteinModels, transcriptsCache: transcripts, genesCache: genes);
        audit.ProteinsCreated += proteins.Count();

        if (effectsDataModel.AffectedTranscripts != null)
        {
            var variantIds = variants.Select(variant => variant.Id).ToArray();
            _affectedTranscriptRepository.RemoveAll(variantIds);

            var affectedTranscriptModels = effectsDataModel.AffectedTranscripts;
            var affectedTranscripts = _affectedTranscriptRepository.CreateAll(affectedTranscriptModels, variants, transcripts);
            audit.AffectedTranscriptsCreated += affectedTranscripts.Count();
        }
    }

    protected override void ProcessModels(IEnumerable<EffectsDataModel> effectsDataModels, ref EffectsWriteAudit audit)
    {
        var variantIds = effectsDataModels.Select(model => model.VariantId);
        var variants = _variantRepository.Find(variantIds);
        audit.Variants.AddRange(variants.Select(variant => variant.Id));

        var geneModels = GetUniqueGeneModels(effectsDataModels.ToArray());
        var genes = _geneRepository.CreateMissing(geneModels);
        audit.GenesCreated += genes.Count();

        var transcriptModels = GetUniqueTranscriptModels(effectsDataModels.ToArray());
        var transcripts = _transcriptRepository.CreateMissing(transcriptModels, genesCache: genes);
        audit.TranscriptsCreated += transcripts.Count();

        var proteinModels = GetUniqueProteinModels(effectsDataModels.ToArray());
        var proteins = _proteinRepository.CreateMissing(proteinModels, transcriptsCache: transcripts, genesCache: genes);
        audit.ProteinsCreated += proteins.Count();

        if (effectsDataModels.Any(consequencesDataModel => consequencesDataModel.AffectedTranscripts != null))
        {
            _affectedTranscriptRepository.RemoveAll(variantIds);

            var affectedTranscriptModels = effectsDataModels
                .Where(annotationsModel => annotationsModel.AffectedTranscripts != null)
                .SelectMany(annotationsModel => annotationsModel.AffectedTranscripts)
                .ToArray();

            var affectedTranscripts = _affectedTranscriptRepository.CreateAll(affectedTranscriptModels, variants, transcripts);

            audit.AffectedTranscriptsCreated += affectedTranscripts.Count();
        }
    }


    protected IEnumerable<GeneModel> GetUniqueGeneModels(params EffectsDataModel[] effectsDataModels)
    {
        return effectsDataModels
            .Where(effectsDataModel => effectsDataModel.AffectedTranscripts != null)
            .SelectMany(effectsDataModel => effectsDataModel.AffectedTranscripts)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Gene)
            .DistinctBy(geneModel => geneModel.Id);
    }

    protected IEnumerable<TranscriptModel> GetUniqueTranscriptModels(params EffectsDataModel[] effectsDataModels)
    {
        return effectsDataModels
            .Where(effectsDataModel => effectsDataModel.AffectedTranscripts != null)
            .SelectMany(effectsDataModel => effectsDataModel.AffectedTranscripts)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Transcript)
            .DistinctBy(transcriptModel => transcriptModel.Id);
    }

    protected IEnumerable<ProteinModel> GetUniqueProteinModels(params EffectsDataModel[] effectsDataModels)
    {
        return effectsDataModels
            .Where(effectsDataModel => effectsDataModel.AffectedTranscripts != null)
            .SelectMany(effectsDataModel => effectsDataModel.AffectedTranscripts)
            .Where(affectedTranscriptModel => affectedTranscriptModel.Protein != null)
            .Select(affectedTranscriptModel => affectedTranscriptModel.Protein)
            .DistinctBy(proteinModel => proteinModel.Id);
    }
}
