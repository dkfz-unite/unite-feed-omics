using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Models.Variants;

namespace Unite.Genome.Annotations.Services.Vep;


public class CopyNumberVariantsAnnotationService
{
    private readonly DomainDbContext _dbContext;
    private readonly AnnotationsDataLoader _dataLoader;


    public CopyNumberVariantsAnnotationService(
        DomainDbContext dbContext,
        IEnsemblOptions ensemblOptions,
        IEnsemblVepOptions ensemblVepOptions
        )
    {
        _dbContext = dbContext;
        _dataLoader = new AnnotationsDataLoader(ensemblOptions, ensemblVepOptions, dbContext);
    }


    public ConsequencesDataModel[] Annotate(long[] variantIds)
    {
        var variants = LoadVariants(variantIds).ToArray();

        var codes = variants.Select(GetVepVariantCode).ToArray();

        var annotations = _dataLoader.LoadData(codes).Result;

        return annotations;
    }


    private IQueryable<Variant> LoadVariants(long[] variantIds)
    {
        return _dbContext.Set<Variant>()
            .Where(entity => variantIds.Contains(entity.Id))
            .Where(entity => entity.TypeId != null)
            .OrderBy(entity => entity.ChromosomeId)
            .ThenBy(entity => entity.Start);
    }

    private string GetVepVariantCode(Variant variant)
    {

        var id = variant.Id;
        var chr = variant.ChromosomeId.ToDefinitionString();
        var start = variant.Start;
        var end = variant.End;
        var type = "CNV";

        return string.Join('\t', chr, start, id, ".", $"<{type}>", ".", ".", $"SVTYPE={type};END={end}", ".");
    }
}
