using Unite.Genome.Feed.Web.Models.Variants.SV.Mappers;

namespace Unite.Genome.Feed.Web.Models.Variants.SV.Converters;

public class SequencingDataModelConverter : Base.Converters.SequencingDataModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public SequencingDataModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.SequencingDataModel<VariantModel> source, Data.Models.AnalysedSampleModel target)
    {
        target.Svs = source.Entries.Select(variant =>
        {
            var variantModel = new Data.Models.Variants.SV.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
