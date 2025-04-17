using Unite.Genome.Feed.Web.Models.Dna.Sm.Mappers;

namespace Unite.Genome.Feed.Web.Models.Dna.Sm.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public AnalysisModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.AnalysisModel<VariantModel> source, Data.Models.SampleModel target)
    {
        target.Sms = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Dna.Sm.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
