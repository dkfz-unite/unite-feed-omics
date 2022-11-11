using Unite.Genome.Feed.Web.Models.Variants.Base.Converters;
using Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Converters;

public class SequencingDataAceSeqModelConverter : SequencingDataModelConverterBase<VariantAceSeqModel>
{
    private readonly VariantAceSeqModelMapper _variantsModelMapper;


    public SequencingDataAceSeqModelConverter() : base()
    {
        _variantsModelMapper = new VariantAceSeqModelMapper();
    }


    protected override void MapVariants(AnalysedSampleModel<VariantAceSeqModel> source, Data.Models.AnalysedSampleModel target)
    {
        target.CopyNumberVariants = source.Variants.Select(variant =>
        {
            var variantModel = new Data.Models.Variants.CNV.VariantModel();

            _variantsModelMapper.Map(variant, variantModel, source.Ploidy);

            return variantModel;

        }).ToArray();
    }
}
