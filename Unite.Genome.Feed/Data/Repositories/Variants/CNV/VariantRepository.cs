using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants.CNV;

namespace Unite.Genome.Feed.Data.Repositories.Variants.CNV;

internal class VariantRepository : VariantRepository<Variant, VariantModel>
{
    public VariantRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }

    public override Variant Find(VariantModel model)
    {
        var entity = _dbContext.Set<Variant>()
            .FirstOrDefault(entity =>
                entity.ChromosomeId == model.Chromosome &&
                entity.Start == model.Start &&
                entity.End == model.End &&
                entity.SvTypeId == model.SvType &&
                entity.CnaTypeId == model.CnaType &&
                entity.HomoDel == model.HomoDel &&
                entity.Loh == model.Loh &&
                entity.C1Mean == model.C1Mean &&
                entity.C2Mean == model.C2Mean &&
                entity.TcnMean == model.TcnMean &&
                entity.C1 == model.C1 &&
                entity.C2 == model.C2 &&
                entity.Tcn == model.Tcn &&
                entity.DhMax == model.DhMax
            );

        return entity;
    }

    protected override void Map(in VariantModel model, ref Variant entity)
    {
        base.Map(model, ref entity);

        entity.SvTypeId = model.SvType;
        entity.CnaTypeId = model.CnaType;
        entity.Loh = model.Loh;
        entity.HomoDel = model.HomoDel;
        entity.C1Mean = model.C1Mean;
        entity.C2Mean = model.C2Mean;
        entity.TcnMean = model.TcnMean;
        entity.C1 = model.C1;
        entity.C2 = model.C2;
        entity.Tcn = model.Tcn;
        entity.TcnRatio = model.TcnRatio;
        entity.DhMax = model.DhMax;
    }
}
