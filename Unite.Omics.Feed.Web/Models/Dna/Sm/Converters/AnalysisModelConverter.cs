using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Binders;
using Unite.Omics.Feed.Web.Models.Dna.Sm.Mappers;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<VariantModel, Data.Models.Dna.Sm.AnalysisModel>
{
    private readonly VariantModelMapper _variantsModelMapper;


    public AnalysisModelConverter() : base()
    {
        _variantsModelMapper = new VariantModelMapper();
    }


    protected override void MapEntries(Base.AnalysisModel<VariantModel> source, Data.Models.Dna.Sm.AnalysisModel target)
    {
        var entriesScope = source.EntriesScope;
        target.EntriesScope = entriesScope.IsNotEmpty()
            ? entriesScope.Select(value => EnumBinder.Bind<SmType>(value).Value).Distinct().ToArray()
            : null;
        target.Entries = source.Entries.Distinct().Select(variant =>
        {
            var variantModel = new Data.Models.Dna.Sm.VariantModel();

            _variantsModelMapper.Map(variant, variantModel);

            return variantModel;

        }).ToArray();
    }
}
