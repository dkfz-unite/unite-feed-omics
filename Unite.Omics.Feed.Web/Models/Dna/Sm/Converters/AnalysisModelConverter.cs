using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Binders;
using Unite.Omics.Feed.Web.Models.Dna.Sm.Mappers;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<VariantModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public AnalysisModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.AnalysisModel<VariantModel> source, Data.Models.SampleModel target)
    {
        var scope = source.TargetSample.Scope;
        target.SmScope = scope.IsNotEmpty()
            ? scope.Select(value => EnumBinder.Bind<SmType>(value).Value).Distinct().ToArray()
            : null;
        target.Sms = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Dna.Sm.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
