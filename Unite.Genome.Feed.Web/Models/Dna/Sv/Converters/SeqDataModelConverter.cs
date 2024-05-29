using Unite.Genome.Feed.Web.Models.Dna.Sv.Mappers;

namespace Unite.Genome.Feed.Web.Models.Dna.Sv.Converters;

public class SeqDataModelConverter : Base.Converters.SeqDataModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public SeqDataModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.SeqDataModel<VariantModel> source, Data.Models.SampleModel target)
    {
        target.Svs = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Dna.Sv.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
