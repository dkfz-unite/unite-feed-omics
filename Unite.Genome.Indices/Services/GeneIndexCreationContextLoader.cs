using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Genome.Indices.Services;

internal class GeneIndexCreationContextLoader
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;


    public GeneIndexCreationContextLoader(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    public GeneIndexCreationContext LoadContext(int geneId)
    {
        var gene = LoadGene(geneId);

        var ssmAffectedTranscriptsCache = LoadAffectedTranscriptsCache<SSM.Variant, SSM.AffectedTranscript>(geneId);
        var cnvAffectedTranscriptsCache = LoadAffectedTranscriptsCache<CNV.Variant, CNV.AffectedTranscript>(geneId);
        var svAffectedTranscriptsCache = LoadAffectedTranscriptsCache<SV.Variant, SV.AffectedTranscript>(geneId);
        var bulkExpressionsCache = LoadExpressionsCache(geneId);

        var ssmAffectedSpecimenIds = LoadAffectedSpecimenIds<SSM.Variant, SSM.VariantEntry, SSM.AffectedTranscript>(ssmAffectedTranscriptsCache);
        var cnvAffectedSpecimenIds = LoadAffectedSpecimenIds<CNV.Variant, CNV.VariantEntry, CNV.AffectedTranscript>(cnvAffectedTranscriptsCache);
        var svAffectedSpecimenIds = LoadAffectedSpecimenIds<SV.Variant, SV.VariantEntry, SV.AffectedTranscript>(svAffectedTranscriptsCache);
        var bulkExpressionSpecimenIds = LoadAffectedSpecimenIds(bulkExpressionsCache);

        var specimenIds = Enumerable.Empty<int>()
            .Union(ssmAffectedSpecimenIds)
            .Union(cnvAffectedSpecimenIds)
            .Union(svAffectedSpecimenIds)
            .Union(bulkExpressionSpecimenIds)
            .ToArray();

        var specimensCache = LoadSpecimensCache(specimenIds);
        var donorsCache = LoadDonorsCache(specimenIds);
        var imagesCache = LoadImagesCache(specimenIds);
        var analysesCache = LoadAnalysesCache(specimenIds);

        return new GeneIndexCreationContext
        {
            Gene = gene,
            SsmAffectedTranscriptsCache = ssmAffectedTranscriptsCache,
            CnvAffectedTranscriptsCache = cnvAffectedTranscriptsCache,
            SvAffectedTranscriptsCache = svAffectedTranscriptsCache,
            BulkExpressionsCache = bulkExpressionsCache,
            SpecimensCache = specimensCache,
            DonorsCache = donorsCache,
            ImagesCache = imagesCache,
            AnalysesCache = analysesCache
        };
    }

    private Gene LoadGene(int geneId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Gene>()
            .AsNoTracking()
            .FirstOrDefault(gene => gene.Id == geneId);
    }

    /// <summary>
    /// Loads selected gene transcripts affected by selected type of the variants.
    /// </summary>
    /// <param name="geneId">Gene id.</param>
    /// <typeparam name="TVariant">Variant type,</typeparam>
    /// <typeparam name="TAffectedTranscript">Affected transcript type.</typeparam>
    /// <returns>Dictionary of affected transcripts cached by variant ids.</returns>
    private Dictionary<long, TAffectedTranscript[]> LoadAffectedTranscriptsCache<TVariant, TAffectedTranscript>(int geneId)
        where TVariant : Variant
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<TAffectedTranscript>()
            .AsNoTracking()
            .Include(affectedTranscript => affectedTranscript.Feature.Protein)
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId)
            .GroupBy(affectedTranscript => affectedTranscript.VariantId) // Translated to SQL GROUP BY
            .ToDictionary(group => group.Key, group => group.ToArray());
    }

    /// <summary>
    /// Loads gene expression for selected gene.
    /// </summary>
    /// <param name="geneId">Gene id.</param>
    /// <returns>Dictionary of bulk gene expressions cached by specimen ids.</returns>
    private Dictionary<int, BulkExpression> LoadExpressionsCache(int geneId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<BulkExpression>()
            .AsNoTracking()
            .Where(expresson => expresson.EntityId == geneId)
            .GroupBy(expression => expression.AnalysedSample.TargetSampleId) // Translated to SQL GROUP BY
            .ToDictionary(group => group.Key, group => group.FirstOrDefault());
    }

    /// <summary>
    /// Loads specimens for selected specimen ids.
    /// </summary>
    /// <param name="specimenids">Specimen ids.</param>
    /// <returns>Dictionary of specimens cached by specimen ids.</returns>
    private Dictionary<int, Specimen> LoadSpecimensCache(IEnumerable<int> specimenids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .Where(specimen => specimenids.Contains(specimen.Id))
            .ToDictionary(specimen => specimen.Id, specimen => specimen);
    }

    /// <summary>
    /// Loads donors for selected specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen ids.</param>
    /// <returns>Dictionary of donors cached by specimen ids.</returns>
    private Dictionary<int, Donor> LoadDonorsCache(IEnumerable<int> specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimensToDonorsMap = dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToDictionary(specimen => specimen.Id, specimen => specimen.DonorId);

        var donorIds = specimensToDonorsMap.Values.Distinct().ToArray();

        var donorsMap = dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeStudies()
            .IncludeProjects()
            .Where(donor => donorIds.Contains(donor.Id))
            .ToDictionary(donor => donor.Id, donor => donor);

        return specimensToDonorsMap.ToDictionary(map => map.Key, map => donorsMap.TryGetValue(map.Value, out var value) ? value : null);
    }

    /// <summary>
    /// Loads images for selected specimen ids.
    /// </summary>
    /// <param name="specimenIds">Specimen ids.</param>
    /// <returns>Dictionary of images cached by specimen ids.</returns>
    private Dictionary<int, Image[]> LoadImagesCache(IEnumerable<int> specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimensToDonorsMap = dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.Tissue.TypeId == TissueType.Tumor)
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToDictionary(specimen => specimen.Id, specimen => specimen.DonorId);

        var donorIds = specimensToDonorsMap.Values.Distinct().ToArray();

        var donorsToImagesMap = dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MriImage)
            .Where(image => donorIds.Contains(image.DonorId))
            .GroupBy(image => image.DonorId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        return specimensToDonorsMap.ToDictionary(map => map.Key, map => donorsToImagesMap.TryGetValue(map.Value, out var value) ? value : null);
    }

    /// <summary>
    /// Loads analyses for selected specimen ids.
    /// </summary>
    /// <param name="specimenIds">Specimen ids.</param>
    /// <returns>Dictionary of analyses cached by specimen ids.</returns>
    private Dictionary<int, AnalysedSample[]> LoadAnalysesCache(IEnumerable<int> specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Include(analysedSample => analysedSample.Analysis)
            .Where(analysedSample => specimenIds.Contains(analysedSample.TargetSampleId))
            .GroupBy(analysedSample => analysedSample.TargetSampleId)
            .ToDictionary(group => group.Key, group => group.ToArray());
    }

    /// <summary>
    /// Loads specimen ids affected by selected type of the variants.
    /// </summary>
    /// <param name="affectedTranscriptsCache">Affected transcripts cache.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantEntry">Variant entry type.</typeparam>
    /// <typeparam name="TAffectedTranscript">Affected transcript type.</typeparam>
    /// <returns>Array of specimen ids if were found or empty array otherwise.</returns>
    private int[] LoadAffectedSpecimenIds<TVariant, TVariantEntry, TAffectedTranscript>(IDictionary<long, TAffectedTranscript[]> affectedTranscriptsCache)
        where TVariant : Variant
        where TVariantEntry : VariantEntry<TVariant>
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
    {
        if (affectedTranscriptsCache?.Any() == true)
        {        
            using var dbContext = _dbContextFactory.CreateDbContext();

            return dbContext.Set<TVariantEntry>()
                .AsNoTracking()
                .Where(entry => affectedTranscriptsCache.Keys.Contains(entry.EntityId))
                .Select(entry => entry.AnalysedSample.TargetSampleId)
                .Distinct()
                .ToArray();
        }

        return [];
    }

    /// <summary>
    /// Loads specimen ids affected by selected type of the expressions.
    /// </summary>
    /// <param name="expressionsCache">Expressions cache.</param>
    /// <typeparam name="TExpression">Expression entry type.</typeparam>
    /// <returns>Array of specimen ids if were found or empty array otherwise.</returns>
    private int[] LoadAffectedSpecimenIds<TExpression>(IDictionary<int, TExpression> expressionsCache)
    {
        if (expressionsCache?.Any() == true)
        {
            return expressionsCache.Keys.ToArray();
        }

        return [];
    }
}
