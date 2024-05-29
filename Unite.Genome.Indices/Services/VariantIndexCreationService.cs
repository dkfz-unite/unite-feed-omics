using Microsoft.EntityFrameworkCore;
using Unite.Essentials.Extensions;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Context.Repositories;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Variants;
using Unite.Mapping;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Genome.Indices.Services;

public class VariantIndexCreationService<TVariant, TVariantEntry>
    where TVariant : Data.Entities.Genome.Analysis.Dna.Variant
    where TVariantEntry : Data.Entities.Genome.Analysis.Dna.VariantEntry<TVariant>
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly SpecimensRepository _specimensRepository;
    private readonly GenesRepository _genesRepository;
    private readonly VariantsRepository _variantsRepository;


    public VariantIndexCreationService(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _specimensRepository = new SpecimensRepository(dbContextFactory);
        _genesRepository = new GenesRepository(dbContextFactory);
        _variantsRepository = new VariantsRepository(dbContextFactory);
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

        var index = VariantIndexMapper.CreateFrom<VariantIndex>(variant);

        index.Specimens = CreateSpecimenIndices(variant.Id);

        // If variant doesn't affect any specimens it should be removed.
        if (index.Specimens.IsEmpty())
        {
            return null;
        }

        index.Data = CreateDataIndex(index);

        return index;
    }

    private DataIndex CreateDataIndex(VariantIndex index)
    {
        index.Data.Ssms = HasSsmIntersections(index);
        index.Data.Cnvs = HasCnvIntersections(index);
        index.Data.Svs = HasSvIntersections(index);
        index.Data.GeneExp = HasGeneExpressions(index);

        return index.Data;
    }

    private TVariant LoadVariant(long variantId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<TVariant>()
            .AsNoTracking()
            .IncludeAffectedTranscripts()
            .FirstOrDefault(variant => variant.Id == variantId);
    }

    private bool HasSsmIntersections(VariantIndex index)
    {
        var specimenIds = index.Specimens.Select(specimen => specimen.Id).ToArray();

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

            var ingoreTypes = new string[] { SV.Enums.SvType.ITX.ToDefinitionString(), SV.Enums.SvType.CTX.ToDefinitionString() };

            if (!ingoreTypes.Contains(index.Sv.Type) && index.Sv.Chromosome == index.Sv.OtherChromosome)
            {
                return HasSsmIntersections(specimenIds, chromosome, index.Sv.End, index.Sv.OtherStart);
            }
        }

        return false;
    }

    private bool HasSsmIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SSM.VariantEntry>()
            .AsNoTracking()
            .FilterBySpecimenId(specimenIds)
            .FilterByRange((int)chromosomeId, start, end)
            .Any();
    }

    private bool HasCnvIntersections(VariantIndex index)
    {
        var specimenIds = index.Specimens.Select(specimen => specimen.Id).ToArray();

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

            var ingoreTypes = new string[] { SV.Enums.SvType.ITX.ToDefinitionString(), SV.Enums.SvType.CTX.ToDefinitionString() };

            if (!ingoreTypes.Contains(index.Sv.Type) && index.Sv.Chromosome == index.Sv.OtherChromosome)
            {
                return HasCnvIntersections(specimenIds, chromosome, index.Sv.End, index.Sv.OtherStart);
            }
        }

        return false;
    }

    private bool HasCnvIntersections(int[] specimenIds, Chromosome chromosomeId, int start, int end)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<CNV.VariantEntry>()
            .AsNoTracking()
            .FilterBySpecimenId(specimenIds)
            .FilterByRange((int)chromosomeId, start, end)
            .Any();
    }

    private bool HasSvIntersections(VariantIndex index)
    {
        var specimenIds = index.Specimens.Select(specimen => specimen.Id).ToArray();

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
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SV.VariantEntry>()
            .AsNoTracking()
            .FilterBySpecimenId(specimenIds)
            .FilterByRange((int)chromosomeId, start, end)
            .Any();
    }

    private bool HasGeneExpressions(VariantIndex index)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = index.Specimens
            .Select(specimen => specimen.Id)
            .ToArray();

        var geneIds = index.GetAffectedFeatures()?
            .Where(affectedFeature => affectedFeature.Gene != null)
            .Select(affectedFeature => (long)affectedFeature.Gene.Id)
            .Distinct()
            .ToArray();

        return dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Where(expression => specimenIds.Contains(expression.Sample.SpecimenId))
            .Any(expression => geneIds.Contains(expression.EntityId));
    }


    private SpecimenIndex[] CreateSpecimenIndices(int variantId)
    {
        var specimens = LoadSpecimens(variantId);

        var indices = specimens.Select(CreateSpecimenIndex);

        return indices.Any() ? indices.ToArray() : null;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
    {
        var index = new SpecimenIndex
        {
            Donor = CreateDonorIndex(specimen.Id, out var diagnosisDate),
            Images = CreateImageIndices(specimen.Id, diagnosisDate),
            Samples = CreateSampleIndices(specimen.Id, diagnosisDate)
        };

        SpecimenIndexMapper.Map(specimen, index, diagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int variantId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _variantsRepository.GetRelatedSpecimens<TVariantEntry, TVariant>([variantId]).Result;
        
        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMaterial()
            .IncludeLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeInterventions()
            .Include(specimen => specimen.SpecimenSamples)
                .ThenInclude(sample => sample.DrugScreenings)
                    .ThenInclude(drugScreening => drugScreening.Entity)
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

    private Donor LoadDonor(int specimenId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var donorIds = _specimensRepository.GetRelatedDonors([specimenId]).Result;

        return dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .Where(donor => donorIds.Contains(donor.Id))
            .FirstOrDefault();
    }


    private ImageIndex[] CreateImageIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(specimenId);

        var indices = images.Select(entity => CreateImageIndex(entity, diagnosisDate));

        return indices.Any() ? indices.ToArray() : null;
    }

    private static ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        return ImageIndexMapper.CreateFrom<ImageIndex>(image, diagnosisDate);
    }

    private Image[] LoadImages(int specimenId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var imageIds = _specimensRepository.GetRelatedImages([specimenId]).Result;

        return dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MriImage)
            .Where(image => imageIds.Contains(image.Id))
            .ToArray();
    }


    private SampleIndex[] CreateSampleIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var samples = LoadAnalyses(specimenId);

        var indices = samples.Select(sample => CreateAnalysisIndex(sample, diagnosisDate));

        return indices.Any() ? indices.ToArray() : null;
    }

    private static SampleIndex CreateAnalysisIndex(Sample sample, DateOnly? diagnosisDate)
    {
        return SampleIndexMapper.CreateFrom<SampleIndex>(sample, diagnosisDate);
    }

    private Sample[] LoadAnalyses(int specimenId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Analysis)
            .Where(sample => sample.SpecimenId == specimenId)
            .ToArray();
    }


    private static Chromosome GetChromosome(string chromosome)
    {
        return Enum.Parse<Chromosome>($"Chr{chromosome}");
    }
}
