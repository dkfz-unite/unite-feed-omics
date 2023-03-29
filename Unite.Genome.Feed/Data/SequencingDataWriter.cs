using Unite.Data.Services;
using Unite.Genome.Feed.Data.Audit;
using Unite.Genome.Feed.Data.Models;

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

            if (analysedSampleModel.Mutations != null)
            {
                WriteMutations(analysedSample.Id, analysedSampleModel.Mutations, ref audit);
            }

            if (analysedSampleModel.CopyNumberVariants != null)
            {
                WriteCopyNumberVariants(analysedSample.Id, analysedSampleModel.CopyNumberVariants, ref audit);
            }

            if (analysedSampleModel.StructuralVariants != null)
            {
                WriteStructuralVariants(analysedSample.Id, analysedSampleModel.StructuralVariants, ref audit);
            }
        }
    }

    private void WriteMutations(int analysedSampleId, IEnumerable<Models.Variants.SSM.VariantModel> variantModels, ref SequencingDataUploadAudit audit)
    {
        var variants = _ssmRepository.CreateMissing(variantModels);

        foreach (var variant in variants)
        {
            audit.Mutations.Add(variant.Id);
            audit.MutationsCreated++;
        }

        _ssmOccurrenceRepository.RemoveAll(analysedSampleId);

        var variantOccurrences = _ssmOccurrenceRepository.CreateAll(analysedSampleId, variantModels, variants);

        foreach (var variantOccurrence in variantOccurrences)
        {
            audit.MutationOccurrences.Add(variantOccurrence.VariantId);
            audit.MutationsAssociated++;
        }
    }

    private void WriteCopyNumberVariants(int analysedSampleId, IEnumerable<Models.Variants.CNV.VariantModel> variantModels, ref SequencingDataUploadAudit audit)
    {
        var variants = _cnvRepository.CreateMissing(variantModels);

        foreach (var variant in variants)
        {
            audit.CopyNumberVariants.Add(variant.Id);
            audit.CopyNumberVariantsCreated++;
        }

        _cnvOccurrenceRepository.RemoveAll(analysedSampleId);

        var variantOccurrences = _cnvOccurrenceRepository.CreateAll(analysedSampleId, variantModels, variants);

        foreach (var variantOccurrence in variantOccurrences)
        {
            audit.CopyNumberVariantOccurrences.Add(variantOccurrence.VariantId);
            audit.CopyNumberVariantsAssociated++;
        }
    }

    private void WriteStructuralVariants(int analysedSampleId, IEnumerable<Models.Variants.SV.VariantModel> variantModels, ref SequencingDataUploadAudit audit)
    {
        var variants = _svRepository.CreateMissing(variantModels);

        foreach (var variant in variants)
        {
            audit.StructuralVariants.Add(variant.Id);
            audit.StructuralVariantsCreated++;
        }

        _svOccurrenceRepository.RemoveAll(analysedSampleId);

        var variantOccurrences = _svOccurrenceRepository.CreateAll(analysedSampleId, variantModels, variants);

        foreach (var variantOccurrence in variantOccurrences)
        {
            audit.StructuralVariantOccurrences.Add(variantOccurrence.VariantId);
            audit.StructuralVariantsAssociated++;
        }
    }
}
