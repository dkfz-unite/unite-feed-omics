using Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Converters;

public class SequencingDataModelConverter : Base.Converters.SequencingDataModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public SequencingDataModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.SequencingDataModel<VariantModel> source, Data.Models.AnalysedSampleModel target)
    {
        target.Cnvs = source.Entries.Select(variant =>
        {
            var variantModel = new Data.Models.Variants.CNV.VariantModel();

            _variantsModelMapper.Map(variant, variantModel, source.TargetSample.Ploidy ?? source.TargetSample.DefaultPloidy);

            return variantModel;

        }).ToArray();
    }
}
