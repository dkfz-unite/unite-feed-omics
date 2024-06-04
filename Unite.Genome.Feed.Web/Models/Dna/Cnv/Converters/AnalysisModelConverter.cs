using Unite.Genome.Feed.Web.Models.Dna.Cnv.Mappers;

namespace Unite.Genome.Feed.Web.Models.Dna.Cnv.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public AnalysisModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.AnalysisModel<VariantModel> source, Data.Models.SampleModel target)
    {
        target.Cnvs = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Dna.Cnv.VariantModel();

            _variantsModelMapper.Map(variant, variantModel, source.TargetSample.Ploidy);

            return variantModel;

        }).ToArray();
    }
}
