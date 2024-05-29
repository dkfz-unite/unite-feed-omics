using Unite.Genome.Feed.Web.Models.Dna.Ssm.Mappers;

namespace Unite.Genome.Feed.Web.Models.Dna.Ssm.Converters;

public class SeqDataModelConverter : Base.Converters.SeqDataModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public SeqDataModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.SeqDataModel<VariantModel> source, Data.Models.SampleModel target)
    {
        target.Ssms = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Dna.Ssm.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
