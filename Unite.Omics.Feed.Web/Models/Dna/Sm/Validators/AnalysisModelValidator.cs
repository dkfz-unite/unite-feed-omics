using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Helpers.Omics.Dna.Sm;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Binders;
using Unite.Omics.Feed.Web.Models.Base.Validators;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Validators;

public class AnalysisModelValidator : AnalysisDataModelValidator<VariantModel>
{
    private static readonly string[] _entriesScope =
    [
        SmType.SNV.ToDefinitionString(),
        SmType.INS.ToDefinitionString(),
        SmType.DEL.ToDefinitionString(),
        SmType.MNV.ToDefinitionString()
    ];


    public AnalysisModelValidator() : base([AnalysisType.WGS, AnalysisType.WES], new VariantModelValidator(), _entriesScope)
    {
        RuleFor(model => model).Custom(ValidateEntriesScope);
    }


    private static void ValidateEntriesScope(AnalysisModel<VariantModel> model, ValidationContext<AnalysisModel<VariantModel>> context)
    {
        var entriesScope = model.EntriesScope;
        var entries = model.Entries;
        if (entriesScope == null || entriesScope.Length == 0 || entries == null)
            return;

        var types = entriesScope.Select(EnumBinder.Bind<SmType>).ToArray();
        if (types.Any(type => type == null))
            return;

        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            if (entry == null || (string.IsNullOrWhiteSpace(entry.Ref) && string.IsNullOrWhiteSpace(entry.Alt)))
                continue;

            var type = TypeDetector.Detect(entry.Ref, entry.Alt);
            if (!types.Contains(type))
                context.AddFailure($"Entries[{i}]", $"Mutation type {type} is outside the declared scope");
        }
    }
}
