using Microsoft.EntityFrameworkCore;
using Unite.Essentials.Extensions;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Indices.Entities.Variants;
using Unite.Mapping;

namespace Unite.Genome.Indices.Services;

public class VariantIndexCreator<TVariant, TVariantEntry>
    where TVariant : Data.Entities.Genome.Analysis.Dna.Variant
    where TVariantEntry : Data.Entities.Genome.Analysis.Dna.VariantEntry<TVariant>
{
    private readonly VariantIndexingCache<TVariant, TVariantEntry> _cache;

    public VariantIndexCreator(VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        _cache = cache;
    }


    public VariantIndex CreateIndex(object key)
    {
        var variantId = (int)key;

        return CreateVariantIndex(variantId);
    }


    private VariantIndex CreateVariantIndex(int variantId)
    {
        var variant = LoadVariant(variantId);

        if (variant == null)
            return null;

        return CreateVariantIndex(variant);
    }

    private VariantIndex CreateVariantIndex(TVariant variant)
    {
        var index = VariantIndexMapper.CreateFrom<VariantIndex>(variant);

        index.Specimens = CreateSpecimenIndices(variant.Id);

        // If variant doesn't affect any specimens it should be removed.
        if (index.Specimens.IsEmpty())
            return null;

        var sampleIds = index.Specimens
            .SelectMany(specimen => specimen.Samples)
            .Select(sample => sample.Id)
            .Distinct()
            .ToArray();

        index.Data.GeneExp = _cache.Expressions.Any(expression => sampleIds.Contains(expression.SampleId));

        return index;
    }

    // private DataIndex CreateDataIndex(VariantIndex index)
    // {
    //     index.Data.Ssms = HasSsmIntersections(index);
    //     index.Data.Cnvs = HasCnvIntersections(index);
    //     index.Data.Svs = HasSvIntersections(index);
    //     index.Data.GeneExp = HasGeneExpressions(index);

    //     return index.Data;
    // }

    private TVariant LoadVariant(int variantId)
    {
        return _cache.Variants.FirstOrDefault(variant => variant.Id == variantId);
    }

    private bool HasSsmIntersections(VariantIndex index)
    {
        // var specimenIds = index.Specimens.Select(specimen => specimen.Id).ToArray();

        // if (index.Ssm != null)
        // {
        //     return true;
        // }
        // else if (index.Cnv != null)
        // {
        //     var chromosome = GetChromosome(index.Cnv.Chromosome);

        //     return HasSsmIntersections(specimenIds, chromosome, index.Cnv.Start, index.Cnv.End);
        // }
        // else if (index.Sv != null)
        // {
        //     var chromosome = GetChromosome(index.Sv.Chromosome);

        //     var ingoreTypes = new string[] { SV.Enums.SvType.ITX.ToDefinitionString(), SV.Enums.SvType.CTX.ToDefinitionString() };

        //     if (!ingoreTypes.Contains(index.Sv.Type) && index.Sv.Chromosome == index.Sv.OtherChromosome)
        //     {
        //         return HasSsmIntersections(specimenIds, chromosome, index.Sv.End, index.Sv.OtherStart);
        //     }
        // }

        // return false;

        return false;
    }

    private bool HasSsmIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        // using var dbContext = _dbContextFactory.CreateDbContext();

        // return dbContext.Set<SSM.VariantEntry>()
        //     .AsNoTracking()
        //     .FilterBySpecimenId(specimenIds)
        //     .FilterByRange((int)chromosomeId, start, end)
        //     .Any();

        return false;
    }

    private bool HasCnvIntersections(VariantIndex index)
    {
        // var specimenIds = index.Specimens.Select(specimen => specimen.Id).ToArray();

        // if (index.Cnv != null)
        // {
        //     return true;
        // }
        // else if (index.Ssm != null)
        // {
        //     var chromosome = GetChromosome(index.Ssm.Chromosome);

        //     return HasCnvIntersections(specimenIds, chromosome, index.Ssm.Start, index.Ssm.End);
        // }
        // else if (index.Sv != null)
        // {
        //     var chromosome = GetChromosome(index.Sv.Chromosome);

        //     var ingoreTypes = new string[] { SV.Enums.SvType.ITX.ToDefinitionString(), SV.Enums.SvType.CTX.ToDefinitionString() };

        //     if (!ingoreTypes.Contains(index.Sv.Type) && index.Sv.Chromosome == index.Sv.OtherChromosome)
        //     {
        //         return HasCnvIntersections(specimenIds, chromosome, index.Sv.End, index.Sv.OtherStart);
        //     }
        // }

        // return false;

        return false;
    }

    private bool HasCnvIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        // using var dbContext = _dbContextFactory.CreateDbContext();

        // return dbContext.Set<CNV.VariantEntry>()
        //     .AsNoTracking()
        //     .FilterBySpecimenId(specimenIds)
        //     .FilterByRange((int)chromosomeId, start, end)
        //     .Any();

        return false;
    }

    private bool HasSvIntersections(VariantIndex index)
    {
        // var specimenIds = index.Specimens.Select(specimen => specimen.Id).ToArray();

        // if (index.Sv != null)
        // {
        //     return true;
        // }
        // else if (index.Ssm != null)
        // {
        //     var chromosome = GetChromosome(index.Ssm.Chromosome);

        //     return HasSvIntersections(specimenIds, chromosome, index.Ssm.Start, index.Ssm.End);
        // }
        // else if (index.Cnv != null)
        // {
        //     var chromosome = GetChromosome(index.Cnv.Chromosome);

        //     return HasSvIntersections(specimenIds, chromosome, index.Cnv.Start, index.Cnv.End);
        // }

        // return false;

        return false;
    }

    private bool HasSvIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        // using var dbContext = _dbContextFactory.CreateDbContext();

        // return dbContext.Set<SV.VariantEntry>()
        //     .AsNoTracking()
        //     .FilterBySpecimenId(specimenIds)
        //     .FilterByRange((int)chromosomeId, start, end)
        //     .Any();

        return false;
    }

    private bool HasGeneExpressions(VariantIndex index)
    {
        // using var dbContext = _dbContextFactory.CreateDbContext();

        // var specimenIds = index.Specimens
        //     .Select(specimen => specimen.Id)
        //     .ToArray();

        // var geneIds = index.GetAffectedFeatures()?
        //     .Where(affectedFeature => affectedFeature.Gene != null)
        //     .Select(affectedFeature => (long)affectedFeature.Gene.Id)
        //     .Distinct()
        //     .ToArray();

        // return dbContext.Set<GeneExpression>()
        //     .AsNoTracking()
        //     .Where(expression => specimenIds.Contains(expression.Sample.SpecimenId))
        //     .Any(expression => geneIds.Contains(expression.EntityId));

        return false;
    }


    private SpecimenIndex[] CreateSpecimenIndices(int variantId)
    {
        var specimens = LoadSpecimens(variantId);

        return specimens.Select(CreateSpecimenIndex).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
    {
        var donors = CreateDonorIndex(specimen.DonorId, out var diagnosisDate);
        var images = CreateImageIndices(specimen.Id, diagnosisDate);
        var samples = CreateSampleIndices(specimen.Id, diagnosisDate);

        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, diagnosisDate);

        index.Donor = donors;
        index.Images = images;
        index.Samples = samples;

        return index;
    }

    private Specimen[] LoadSpecimens(int variantId)
    {
        var sampleIds = _cache.Entries
            .Where(entry => entry.EntityId == variantId)
            .Select(entry => entry.SampleId)
            .Distinct()
            .ToArray();

        var specimenIds = _cache.Samples
            .Where(sample => sampleIds.Contains(sample.Id))
            .Select(sample => sample.SpecimenId)
            .Distinct()
            .ToArray();

        return _cache.Specimens
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();
    }


    private DonorIndex CreateDonorIndex(int specimenId, out DateOnly? diagnosisDate)
    {
        var donor = LoadDonor(specimenId);

        if (donor == null)
        {
            diagnosisDate = null;
            return null;
        }

        diagnosisDate = donor.ClinicalData?.DiagnosisDate;
        return CreateDonorIndex(donor);
    }

    private static DonorIndex CreateDonorIndex(Donor donor)
    {
        return DonorIndexMapper.CreateFrom<DonorIndex>(donor);
    }

    private Donor LoadDonor(int donorId)
    {
        return _cache.Donors.FirstOrDefault(donor => donor.Id == donorId);
    }


    private ImageIndex[] CreateImageIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(specimenId);

        return images.Select(entity => CreateImageIndex(entity, diagnosisDate)).ToArrayOrNull();
    }

    private static ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        return ImageIndexMapper.CreateFrom<ImageIndex>(image, diagnosisDate);
    }

    private Image[] LoadImages(int donorId)
    {
        return _cache.Images.Where(image => image.DonorId == donorId).ToArray();
    }


    private SampleIndex[] CreateSampleIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var samples = LoadSamples(specimenId);

        return samples.Select(sample => CreateSampleIndex(sample, diagnosisDate)).ToArrayOrNull();
    }

    private static SampleIndex CreateSampleIndex(Sample sample, DateOnly? diagnosisDate)
    {
        return SampleIndexMapper.CreateFrom<SampleIndex>(sample, diagnosisDate);
    }

    private Sample[] LoadSamples(int specimenId)
    {
        return _cache.Samples.Where(sample => sample.SpecimenId == specimenId).ToArray();
    }


    // private static Chromosome GetChromosome(string chromosome)
    // {
    //     return Enum.Parse<Chromosome>($"Chr{chromosome}");
    // }
}
