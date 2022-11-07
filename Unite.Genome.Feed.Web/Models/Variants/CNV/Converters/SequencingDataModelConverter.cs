using Unite.Genome.Feed.Web.Models.Variants.Base.Converters;
using Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Converters;

public class SequencingDataModelConverter : SequencingDataModelConverterBase<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public SequencingDataModelConverter() : base()
    {
        _variantsModelMapper = new Mappers.VariantModelMapper();
    }


    protected override void MapVariants(AnalysedSampleModel<VariantModel> source, Data.Models.AnalysedSampleModel target)
    {
        target.CopyNumberVariants = source.Variants.Select(variant =>
        {
            var variantModel = new Data.Models.Variants.CNV.VariantModel();

            _variantsModelMapper.Map(variant, variantModel, source.Ploidy ?? 2);

            return variantModel;

        }).ToArray();
    }
}
