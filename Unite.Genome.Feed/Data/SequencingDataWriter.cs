using Unite.Data.Services;
using Unite.Genome.Feed.Data.Audit;
using Unite.Genome.Feed.Data.Extensions;
using Unite.Genome.Feed.Data.Models;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;

namespace Unite.Genome.Feed.Data;

public class SequencingDataWriter : DataWriter<AnalysisModel, SequencingDataUploadAudit>
{
    private readonly Repositories.AnalysisRepository _analysisRepository;
    private readonly Repositories.AnalysedSampleRepository _analysedSampleRepository;
    private readonly Repositories.Variants.SSM.VariantRepository _ssmRepository;
    private readonly Repositories.Variants.SSM.VariantOccurrenceRepository _ssmOccurrenceRepository;
    private readonly Repositories.Variants.CNV.VariantRepository _cnvRepository;
    private readonly Repositories.Variants.CNV.VariantOccurrenceRepository _cnvOccurrenceRepository;
    private readonly Repositories.Variants.SV.VariantRepository _svRepository;
    private readonly Repositories.Variants.SV.VariantOccurrenceRepository _svOccurrenceRepository;


    public SequencingDataWriter(DomainDbContext dbContext) : base(dbContext)
    {
        _analysisRepository = new Repositories.AnalysisRepository(dbContext);
        _analysedSampleRepository = new Repositories.AnalysedSampleRepository(dbContext);
        _ssmRepository = new Repositories.Variants.SSM.VariantRepository(dbContext);
        _ssmOccurrenceRepository = new Repositories.Variants.SSM.VariantOccurrenceRepository(dbContext, _ssmRepository);
        _cnvRepository = new Repositories.Variants.CNV.VariantRepository(dbContext);
        _cnvOccurrenceRepository = new Repositories.Variants.CNV.VariantOccurrenceRepository(dbContext, _cnvRepository);
        _svRepository = new Repositories.Variants.SV.VariantRepository(dbContext);
        _svOccurrenceRepository = new Repositories.Variants.SV.VariantOccurrenceRepository(dbContext, _svRepository);
    }


    protected override void ProcessModel(AnalysisModel analysisModel, ref SequencingDataUploadAudit audit)
    {
        var analysis = _analysisRepository.FindOrCreate(analysisModel);

        foreach (var analysedSampleModel in analysisModel.AnalysedSamples)
        {
            var analysedSample = _analysedSampleRepository.FindOrCreate(analysis.Id, analysedSampleModel);

            if (analysedSampleModel.Mutations?.Any() == true)
            {
                WriteMutations(analysedSample.Id, analysedSampleModel.Mutations, ref audit);
            }

            if (analysedSampleModel.CopyNumberVariants?.Any() == true)
            {
                WriteCopyNumberVariants(analysedSample.Id, analysedSampleModel.CopyNumberVariants, ref audit);
            }

            if (analysedSampleModel.StructuralVariants?.Any() == true)
            {
                WriteStructuralVariants(analysedSample.Id, analysedSampleModel.StructuralVariants, ref audit);
            }
        }
    }

    private void WriteMutations(int analysedSampleId, IEnumerable<Models.Variants.SSM.VariantModel> variantModels, ref SequencingDataUploadAudit audit)
    {
        var variants = _ssmRepository.CreateMissing(variantModels);
        audit.Mutations.AddRange(variants.Select(variant => variant.Id));
        audit.MutationsCreated += variants.Count();

        var existing = _dbContext.Set<Unite.Data.Entities.Genome.Variants.SSM.VariantOccurrence>().Count(o => o.AnalysedSampleId == analysedSampleId);
        // _ssmOccurrenceRepository.RemoveAll(analysedSampleId);

        var variantOccurrences = _ssmOccurrenceRepository.CreateMissing(analysedSampleId, variantModels, variants);
        audit.MutationOccurrences.AddRange(variantOccurrences.Select(occurrence => occurrence.VariantId));
        audit.MutationsAssociated += variantOccurrences.Count();
        var created = _dbContext.Set<Unite.Data.Entities.Genome.Variants.SSM.VariantOccurrence>().Count(o => o.AnalysedSampleId == analysedSampleId);
    }

    private void WriteCopyNumberVariants(int analysedSampleId, IEnumerable<Models.Variants.CNV.VariantModel> variantModels, ref SequencingDataUploadAudit audit)
    {
        var variants = _cnvRepository.CreateMissing(variantModels);
        var predicate = GetCnvExcludePredicate();
        audit.CopyNumberVariants.AddRange(variants.Where(predicate).Select(variant => variant.Id));
        audit.CopyNumberVariantsCreated += variants.Count();

        // _cnvOccurrenceRepository.RemoveAll(analysedSampleId);

        var variantOccurrences = _cnvOccurrenceRepository.CreateMissing(analysedSampleId, variantModels, variants);
        audit.CopyNumberVariantOccurrences.AddRange(variantOccurrences.Select(occurrence => occurrence.VariantId));
        audit.CopyNumberVariantsAssociated += variantOccurrences.Count();
    }

    private void WriteStructuralVariants(int analysedSampleId, IEnumerable<Models.Variants.SV.VariantModel> variantModels, ref SequencingDataUploadAudit audit)
    {
        var variants = _svRepository.CreateMissing(variantModels);
        audit.StructuralVariants.AddRange(variants.Select(variant => variant.Id));
        audit.StructuralVariantsCreated += variants.Count();

        // _svOccurrenceRepository.RemoveAll(analysedSampleId);

        var variantOccurrences = _svOccurrenceRepository.CreateMissing(analysedSampleId, variantModels, variants);
        audit.StructuralVariantOccurrences.AddRange(variantOccurrences.Select(occurrence => occurrence.VariantId));
        audit.StructuralVariantsAssociated += variantOccurrences.Count();
    }


    /// <summary>
    /// Returns a predicate that excludes CNVs that are neutral and not LOH.
    /// </summary>
    /// <returns>Predicate function.</returns>
    private static Func<CNV.Variant, bool> GetCnvExcludePredicate()
    {
        return variant => !(variant.TypeId == CNV.Enums.CnvType.Neutral && variant.Loh == false);
    }
}
