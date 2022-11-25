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
        _dataLoader = new AnnotationsDataLoader(ensemblOptions, ensemblVepOptions, dbContext);
    }


    public void Annotate(long[] variantIds, out ConsequencesDataUploadAudit audit)
    {
        var variants = LoadVariants(variantIds).ToArray();

        var codes = variants.SelectMany(GetVepVariantCodes).ToArray();

        var annotations = _dataLoader.LoadData(codes).Result;

        _dataWriter.SaveData(annotations, out audit);
    }


    private IQueryable<Variant> LoadVariants(long[] variantIds)
    {
        return _dbContext.Set<Variant>()
            .Where(entity => variantIds.Contains(entity.Id))
            .Where(entity => entity.TypeId != SvType.COM)
            .OrderBy(entity => entity.ChromosomeId)
            .ThenBy(entity => entity.Start);
    }

    private IEnumerable<string> GetVepVariantCodes(Variant variant)
    {
        var chr1 = variant.ChromosomeId.ToDefinitionString();
        var start1 = variant.Start;
        var end1 = variant.End;

        var chr2 = variant.OtherChromosomeId.ToDefinitionString();
        var start2 = variant.OtherStart;
        var end2 = variant.OtherEnd;

        if (variant.TypeId == SvType.ITX && variant.TypeId == SvType.CTX)
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
            var type = variant.TypeId == SvType.TDUP ? "DUP:TANDEM" : variant.TypeId.ToDefinitionString();

            yield return string.Join('\t', chr, start, id, ".", $"<{type}>", ".", ".", $"SVTYPE={type};END={end}", ".");
        }
    }
}
