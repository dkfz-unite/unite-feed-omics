using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Models.Variants;

namespace Unite.Genome.Annotations.Services.Vep;


public class MutationsAnnotationService
{
    private readonly DomainDbContext _dbContext;
    private readonly AnnotationsDataLoader _dataLoader;


    public MutationsAnnotationService(
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
        var variants = LoadVariants(variantIds);

        var codes = variants.Select(GetVepVariantCode).ToArray();

        var annotations = _dataLoader.LoadData(codes).Result;

        return annotations;
    }


    private IQueryable<Variant> LoadVariants(long[] variantIds)
    {
        return _dbContext.Set<Variant>()
            .Where(entity => variantIds.Contains(entity.Id))
            .OrderBy(entity => entity.ChromosomeId)
            .ThenBy(entity => entity.Start);
    }

    private string GetVepVariantCode(Variant variant)
    {
        var id = variant.Id.ToString();
        var chromosome = variant.ChromosomeId.ToDefinitionString();
        var start = variant.Ref != null ? variant.Start : variant.End + 1;
        var end = variant.End;
        var referenceBase = variant.Ref ?? "-";
        var alternateBase = variant.Alt ?? "-";

        return $"{chromosome} {start} {end} {referenceBase}/{alternateBase} + {id}";
    }
}
