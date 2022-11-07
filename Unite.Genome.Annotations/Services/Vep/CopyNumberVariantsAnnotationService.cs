using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Data;
using Unite.Genome.Annotations.Data.Models;
using Unite.Genome.Annotations.Data.Repositories;
using Unite.Genome.Annotations.Data.Repositories.Variants.CNV;

namespace Unite.Genome.Annotations.Services.Vep;


public class CopyNumberVariantsAnnotationService
{
    private readonly DomainDbContext _dbContext;
    private readonly VariantRepository<Variant> _variantRepository;
    private readonly AffectedTranscriptRepository<Variant, AffectedTranscript> _affectedTranscriptRepository;
    private readonly ConsequencesDataWriter<Variant, AffectedTranscript> _dataWriter;
    private readonly AnnotationsDataLoader _dataLoader;


    public CopyNumberVariantsAnnotationService(
        DomainDbContext dbContext,
        IEnsemblOptions ensemblOptions,
        IEnsemblVepOptions ensemblVepOptions
        )
    {
        _dbContext = dbContext;
        _variantRepository = new VariantRepository<Variant>(dbContext);
        _affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext);
        _dataWriter = new ConsequencesDataWriter<Variant, AffectedTranscript>(dbContext, _variantRepository, _affectedTranscriptRepository);
        _dataLoader = new AnnotationsDataLoader(ensemblOptions, ensemblVepOptions);
    }


    public void Annotate(long[] variantIds, out ConsequencesDataUploadAudit audit)
    {
        var variants = LoadVariants(variantIds).ToArray();

        var codes = variants.Select(GetVepVariantCode).ToArray();

        var annotations = _dataLoader.LoadData(codes).Result;

        _dataWriter.SaveData(annotations, out audit);
    }


    private IQueryable<Variant> LoadVariants(long[] variantIds)
    {
        return _dbContext.Set<Variant>()
            .Where(entity => variantIds.Contains(entity.Id))
            .Where(entity => entity.CnaTypeId != null)
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
