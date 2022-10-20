using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Data.Entities.Genome.Variants.SV.Enums;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Data;
using Unite.Genome.Annotations.Data.Models;
using Unite.Genome.Annotations.Data.Repositories;
using Unite.Genome.Annotations.Data.Repositories.Variants.SV;

namespace Unite.Genome.Annotations.Services.Vep;


public class StructuralVariantsAnnotationService
{
    private readonly DomainDbContext _dbContext;
    private readonly VariantRepository<Variant> _variantRepository;
    private readonly AffectedTranscriptRepository<Variant, AffectedTranscript> _affectedTranscriptRepository;
    private readonly ConsequencesDataWriter<Variant, AffectedTranscript> _dataWriter;
    private readonly AnnotationsDataLoader _dataLoader;


    public StructuralVariantsAnnotationService(
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
        var variants = LoadVariants(variantIds);

        var codes = variants.Select(GetVepVariantCode).ToArray();

        var annotations = _dataLoader.LoadData(codes).Result;

        _dataWriter.SaveData(annotations, out audit);
    }


    private IQueryable<Variant> LoadVariants(long[] variantIds)
    {
        var supportedTypes = new SvType?[] { SvType.DUP, SvType.INS, SvType.DEL };

        return _dbContext.Set<Variant>()
            .Where(entity => supportedTypes.Contains(entity.TypeId));
    }

    private string GetVepVariantCode(Variant variant)
    {
        var id = variant.Id.ToString();
        var chromosome = variant.ChromosomeId.ToDefinitionString();
        var start = variant.Start;
        var end = variant.End;
        var type = GetVepVariantType(variant);

        return $"{chromosome} {start} {end} {type} + {id}";
    }

    private string GetVepVariantType(Variant variant)
    {
        if (variant.TypeId == SvType.DUP)
        {
            var isTandem = variant.NewChromosomeId == variant.ChromosomeId && variant.NewStart == variant.End + 1;

            return isTandem ? "TDUP" : "DUP";
        }
        else if (variant.TypeId == SvType.INS)
        {
            return "INS";
        }
        else if (variant.TypeId == SvType.DEL)
        {
            return "DEL";
        }
        else
        {
            throw new NotSupportedException($"Structural variant type '{variant.TypeId.ToDefinitionString()}' is not supported by Ensembl VEP");
        }
    }
}
