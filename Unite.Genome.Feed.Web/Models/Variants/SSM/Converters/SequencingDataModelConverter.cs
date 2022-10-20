using Unite.Genome.Feed.Web.Models.Variants.Base.Converters;
using Unite.Genome.Feed.Web.Models.Variants.SSM.Mappers;

namespace Unite.Genome.Feed.Web.Models.Variants.SSM.Converters;

public class SequencingDataModelConverter : SequencingDataModelConverterBase<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public SequencingDataModelConverter() : base()
    {
        _variantsModelMapper = new Mappers.VariantModelMapper();
    }


    protected override void MapVariants(AnalysedSampleModel<VariantModel> source, Data.Models.AnalysedSampleModel target)
    {
        target.Mutations = source.Variants.Select(variant =>
        {
            var variantModel = new Data.Models.Variants.SSM.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
