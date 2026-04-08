using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Models.Dna;

namespace Unite.Omics.Annotations.Services.Vep;


public class SvsAnnotationService
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly AnnotationsDataLoader _dataLoader;


    public SvsAnnotationService(
        IDbContextFactory<DomainDbContext> dbContextFactory,
        IEnsemblDataOptions ensemblOptions,
        IEnsemblVepOptions ensemblVepOptions
        )
    {
        _dbContextFactory = dbContextFactory;
        _dataLoader = new AnnotationsDataLoader(ensemblOptions, ensemblVepOptions, dbContextFactory);
    }


    public EffectsDataModel[] Annotate(int[] variantIds)
    {
        var variants = LoadVariants(variantIds);

        var codes = variants.SelectMany(GetVepVariantCodes).ToArray();

        var annotations = _dataLoader.LoadData(codes).Result;

        return annotations;
    }


    private Variant[] LoadVariants(int[] variantIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Variant>()
            .AsNoTracking()
            .Where(entity => variantIds.Contains(entity.Id))
            .Where(entity => entity.TypeId != SvType.COM)
            .OrderBy(entity => entity.ChromosomeId)
            .ThenBy(entity => entity.Start)
            .ToArray();
    }

    private IEnumerable<string> GetVepVariantCodes(Variant variant)
    {
        var chr1 = variant.ChromosomeId.ToDefinitionString();
        var start1 = variant.Start;
        var end1 = variant.End;

        var chr2 = variant.OtherChromosomeId.ToDefinitionString();
        var start2 = variant.OtherStart;
        var end2 = variant.OtherEnd;

        if (variant.TypeId == SvType.ITX || variant.TypeId == SvType.CTX)
        {
            var mate1s = $"{variant.Id}.1S";
            var mate1e = $"{variant.Id}.1E";
            var mate2s = $"{variant.Id}.2S";
            var mate2e = $"{variant.Id}.2E";

            if (variant.Inverted == null && variant.Inverted == false)
            {
                // mate1s-mate2e; mate2s-mate1e
                yield return string.Join('\t', chr1, start1, mate1s, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate2e}", ".");
                yield return string.Join('\t', chr1, end1, mate1e, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate2s}", ".");
                yield return string.Join('\t', chr2, start2, mate2s, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate1e}", ".");
                yield return string.Join('\t', chr2, end2, mate2e, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate1s}", ".");
            }
            else
            {
                // mate1s-mate2s; mate1e-mate2e
                yield return string.Join('\t', chr1, start1, mate1s, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate2s}", ".");
                yield return string.Join('\t', chr1, end1, mate1e, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate2e}", ".");
                yield return string.Join('\t', chr2, start2, mate2s, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate1s}", ".");
                yield return string.Join('\t', chr2, end2, mate2e, ".", ".", ".", ".", $"SVTYPE=BND;MATEID={mate1e}", ".");
            }
        }
        else
        {
            var id = variant.Id;
            var positions = new int[] { start1, end1, start2, end2 };
            var start = positions.Min();
            var end = positions.Max();
            var chr = variant.ChromosomeId.ToDefinitionString();
            var type = variant.TypeId.ToDefinitionString();

            yield return string.Join('\t', chr, start, id, ".", $"<{type}>", ".", ".", $"SVTYPE={type};END={end}", ".");
        }
    }
}
