using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using Unite.Genome.Feed.Data.Models.Dna.Cnv;

namespace Unite.Genome.Feed.Data.Repositories.Dna.Cnv;

public class VariantRepository : VariantRepository<Variant, VariantModel>
{
    public VariantRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }

    protected override Expression<Func<Variant, bool>> GetModelPredicate(VariantModel model)
    {
        return (entity) =>
            entity.ChromosomeId == model.Chromosome &&
            entity.Start == model.Start &&
            entity.End == model.End &&
            entity.TypeId == model.Type &&
            entity.Del == model.Del &&
            entity.Loh == model.Loh &&
            entity.C1Mean == model.C1Mean &&
            entity.C2Mean == model.C2Mean &&
            entity.TcnMean == model.TcnMean &&
            entity.C1 == model.C1 &&
            entity.C2 == model.C2 &&
            entity.Tcn == model.Tcn &&
            entity.DhMax == model.DhMax;
    }

    protected override void Map(in VariantModel model, ref Variant entity)
    {
        base.Map(model, ref entity);

        entity.TypeId = model.Type;
        entity.Loh = model.Loh;
        entity.Del = model.Del;
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
