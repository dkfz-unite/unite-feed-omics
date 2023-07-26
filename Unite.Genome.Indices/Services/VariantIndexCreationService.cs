using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Genome.Indices.Services.Mappers;
using Unite.Indices.Entities.Variants;
using Unite.Indices.Services;

using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Genome.Indices.Services;

public class VariantIndexCreationService<TVariant, TVariantOccurrence> : IIndexCreationService<VariantIndex>
    where TVariant : Data.Entities.Genome.Variants.Variant
    where TVariantOccurrence : Data.Entities.Genome.Variants.VariantOccurrence<TVariant>
{
    private readonly DomainDbContext _dbContext;
    private readonly VariantIndexMapper _variantIndexMapper;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;
    private readonly SampleIndexMapper _sampleIndexMapper;


    public VariantIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _variantIndexMapper = new VariantIndexMapper();
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
        _sampleIndexMapper = new SampleIndexMapper();
    }


    public VariantIndex CreateIndex(object key)
    {
        var variantId = (long)key;

        return CreateVariantIndex(variantId);
    }


    private VariantIndex CreateVariantIndex(long variantId)
    {
        var variant = LoadVariant(variantId);

        var index = CreateVariantIndex(variant);

        return index;
    }

    private VariantIndex CreateVariantIndex(TVariant variant)
    {
        if (variant == null)
        {
            return null;
        }

        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index);

        index.Samples = CreateSampleIndices(variant.Id);
        index.Data = CreateDataIndex(index);

        return index;
    }

    private DataIndex CreateDataIndex(VariantIndex index)
    {
        var dataIndex = index.Data;

        dataIndex.Ssms = HasSsmIntersections(index);
        dataIndex.Cnvs = HasCnvIntersections(index);
        dataIndex.Svs = HasSvIntersections(index);
        dataIndex.GeneExp = HasGeneExpressions(index);

        return dataIndex;
    }

    private TVariant LoadVariant(long variantId)
    {
        var variant = _dbContext.Set<TVariant>()
            .AsNoTracking()
            .IncludeAffectedTranscripts()
            .FirstOrDefault(variant => variant.Id == variantId);

        return variant;
    }

    private bool HasSsmIntersections(VariantIndex index)
    {
        var specimenIds = index.Samples.Select(sample => sample.Specimen.Id).ToArray();

        if (index.Ssm != null)
        {
            return true;
        }
        else if (index.Cnv != null)
        {
            var chromosome = GetChromosome(index.Cnv.Chromosome);

            return HasSsmIntersections(specimenIds, chromosome, index.Cnv.Start, index.Cnv.End);
        }
        else if (index.Sv != null)
        {
            var chromosome = GetChromosome(index.Sv.Chromosome);

            var ingoreTypes = new string[] 
            {
                SV.Enums.SvType.ITX.ToDefinitionString(), 
                SV.Enums.SvType.CTX.ToDefinitionString(), 
                SV.Enums.SvType.COM.ToDefinitionString() 
            };

            if (!ingoreTypes.Contains(index.Sv.Type))
            {
                return HasSsmIntersections(specimenIds, chromosome, index.Sv.Start, index.Sv.OtherStart);
            }
        }

        return false;
    }

    private bool HasSsmIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        return _dbContext.Set<SSM.VariantOccurrence>().AsNoTracking()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Any(occurrence => occurrence.Variant.ChromosomeId == chromosomeId && (
                (occurrence.Variant.End >= start && occurrence.Variant.End <= end) ||
                (occurrence.Variant.Start >= start && occurrence.Variant.Start <= end) ||
                (occurrence.Variant.Start >= start && occurrence.Variant.End <= end) ||
                (occurrence.Variant.Start <= start && occurrence.Variant.End >= end)
            ));
    }

    private bool HasCnvIntersections(VariantIndex index)
    {
        var specimenIds = index.Samples.Select(sample => sample.Specimen.Id).ToArray();

        if (index.Cnv != null)
        {
            return true;
        }
        else if (index.Ssm != null)
        {
            var chromosome = GetChromosome(index.Ssm.Chromosome);

            return HasCnvIntersections(specimenIds, chromosome, index.Ssm.Start, index.Ssm.End);
        }
        else if (index.Sv != null)
        {
            var chromosome = GetChromosome(index.Sv.Chromosome);

            var ingoreTypes = new string[] 
            { 
                SV.Enums.SvType.ITX.ToDefinitionString(), 
                SV.Enums.SvType.CTX.ToDefinitionString(), 
                SV.Enums.SvType.COM.ToDefinitionString() 
            };

            if (!ingoreTypes.Contains(index.Sv.Type))
            {
                return HasCnvIntersections(specimenIds, chromosome, index.Sv.Start, index.Sv.OtherStart);
            }
        }

        return false;
    }

    private bool HasCnvIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        return _dbContext.Set<CNV.VariantOccurrence>().AsNoTracking()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Any(occurrence => occurrence.Variant.ChromosomeId == chromosomeId && (
                (occurrence.Variant.End >= start && occurrence.Variant.End <= end) ||
                (occurrence.Variant.Start >= start && occurrence.Variant.Start <= end) ||
                (occurrence.Variant.Start >= start && occurrence.Variant.End <= end) ||
                (occurrence.Variant.Start <= start && occurrence.Variant.End >= end)
            ));
    }

    private bool HasSvIntersections(VariantIndex index)
    {
        var specimenIds = index.Samples.Select(sample => sample.Specimen.Id).ToArray();

        if (index.Sv != null)
        {
            return true;
        }
        else if (index.Ssm != null)
        {
            var chromosome = GetChromosome(index.Ssm.Chromosome);

            return HasSvIntersections(specimenIds, chromosome, index.Ssm.Start, index.Ssm.End);
        }
        else if (index.Cnv != null)
        {
            var chromosome = GetChromosome(index.Cnv.Chromosome);

            return HasSvIntersections(specimenIds, chromosome, index.Cnv.Start, index.Cnv.End);
        }

        return false;
    }

    private bool HasSvIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        return _dbContext.Set<SV.VariantOccurrence>().AsNoTracking()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Any(occurrence => occurrence.Variant.ChromosomeId == chromosomeId && (
                (occurrence.Variant.OtherStart >= start && occurrence.Variant.OtherStart <= end) ||
                (occurrence.Variant.Start >= start && occurrence.Variant.Start <= end) ||
                (occurrence.Variant.Start >= start && occurrence.Variant.OtherStart <= end) ||
                (occurrence.Variant.Start <= start && occurrence.Variant.OtherStart >= end)
            ));
    }

    private bool HasGeneExpressions(VariantIndex index)
    {
        var specimenIds = index.Samples.Select(sample => sample.Specimen.Id).ToArray();

        var genes = index.GetAffectedFeatures()?
            .Where(affectedFeature => affectedFeature.Gene != null)
            .Select(affectedFeature => affectedFeature.Gene.Id)
            .Distinct()
            .ToArray();

        return genes != null && _dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Where(expression => specimenIds.Contains(expression.AnalysedSample.Sample.SpecimenId))
            .Any(expression => genes.Contains(expression.GeneId));
    }


    private SampleIndex[] CreateSampleIndices(long variantId)
    {
        var samples = LoadSamples(variantId);

        if (samples == null)
        {
            return null;
        }

        var indices = samples
            .Select(sample => CreateSampleIndex(sample.Sample, sample.Analyses))
            .ToArray();
        
        return indices;
    }

    private SampleIndex CreateSampleIndex(Sample sample, Analysis[] analyses)
    {
        var index = new SampleIndex();

        index.Donor = CreateDonorIndex(sample.SpecimenId, out var donor);

        index.Specimen = CreateSpecimenIndex(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        index.Images = CreateImageIndices(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        _sampleIndexMapper.Map(sample, analyses, index, donor.ClinicalData?.DiagnosisDate);

        return index;
    }

    private (Sample Sample, Analysis[] Analyses)[] LoadSamples(long variantId)
    {
        var analysedSampleIds = _dbContext.Set<TVariantOccurrence>()
            .AsNoTracking()
            .Where(occurrence => occurrence.VariantId == variantId)
            .Select(occurrence => occurrence.AnalysedSampleId)
            .Distinct()
            .ToArray();

        var analysedSamples = _dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Include(analysedSample => analysedSample.Sample)
            .Include(analysedSample => analysedSample.Analysis)
            .Where(analysedSample => analysedSampleIds.Contains(analysedSample.Id))
            .ToArray();

        var samples = analysedSamples
            .GroupBy(analysedSample => analysedSample.SampleId)
            .Select(group => (group.First().Sample, group.Select(sample => sample.Analysis).ToArray()))
            .ToArray();

        return samples;
    }


    private SpecimenIndex CreateSpecimenIndex(int specimenId, DateOnly? diagnosisDate)
    {
        var specimen = LoadSpecimen(specimenId);

        if (specimen == null)
        {
            return null;
        }

        var index = CreateSpecimenIndex(specimen, diagnosisDate);

        return index;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index = new SpecimenIndex();

        _specimenIndexMapper.Map(specimen, index, diagnosisDate);

        return index;
    }

    private Specimen LoadSpecimen(int specimenId)
    {
        var specimen = _dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .FirstOrDefault(specimen => specimen.Id == specimenId);

        return specimen;
    }


    private DonorIndex CreateDonorIndex(int specimenId, out Donor donor)
    {
        donor = LoadDonor(specimenId);

        if (donor == null)
        {
            return null;
        }

        var index = CreateDonorIndex(donor);

        return index;
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        var index = new DonorIndex();

        _donorIndexMapper.Map(donor, index);

        return index;
    }

    private Donor LoadDonor(int specimenId)
    {
        var donorId = _dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.Id == specimenId)
            .Select(specimen => specimen.DonorId)
            .FirstOrDefault();

        var donor = _dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .Where(donor => donor.Id == donorId)
            .FirstOrDefault();

        return donor;
    }


    private ImageIndex[] CreateImageIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(specimenId);

        if (images == null)
        {
            return null;
        }

        var indices = images
            .Select(image => CreateImageIndex(image, diagnosisDate))
            .ToArray();

        return indices;
    }

    private ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        var index = new ImageIndex();

        _imageIndexMapper.Map(image, index, diagnosisDate);

        return index;
    }

    private Image[] LoadImages(int specimenId)
    {
        var donorId = _dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.Tissue.TypeId == TissueType.Tumor)
            .Where(specimen => specimen.Id == specimenId)
            .Select(specimen => specimen.DonorId)
            .FirstOrDefault();

        var images = _dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MriImage)
            .Where(image => image.DonorId == donorId)
            .ToArray();

        return images;
    }


    private static Chromosome GetChromosome(string chromosome)
    {
        return Enum.Parse<Chromosome>($"Chr{chromosome}");
    }
}
