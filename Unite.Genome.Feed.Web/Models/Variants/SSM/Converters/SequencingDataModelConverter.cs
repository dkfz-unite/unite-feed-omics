using Unite.Genome.Feed.Web.Models.Variants.SSM.Mappers;

namespace Unite.Genome.Feed.Web.Models.Variants.SSM.Converters;

public class SequencingDataModelConverter : Base.Converters.SequencingDataModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public SequencingDataModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.SequencingDataModel<VariantModel> source, Data.Models.AnalysedSampleModel target)
    {
        target.Ssms = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Variants.SSM.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
