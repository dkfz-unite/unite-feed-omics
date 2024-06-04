using Unite.Genome.Feed.Web.Models.Dna.Sv.Mappers;

namespace Unite.Genome.Feed.Web.Models.Dna.Sv.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public AnalysisModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.AnalysisModel<VariantModel> source, Data.Models.SampleModel target)
    {
        target.Svs = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Dna.Sv.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
