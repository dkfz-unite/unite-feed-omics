using FluentValidation;
using Unite.Data.Constants;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Models.RnaSc.Validators;

public class AnalysisModelValidator : Base.Validators.AnalysisResourcesModelValidator
{
    private static readonly (string Name, string Format)[] _requiredResources =
    [
        ("matrix.mtx.gz", FileTypes.Sequence.Mtx),
        ("features.tsv.gz", FileTypes.General.Tsv),
        ("barcodes.tsv.gz", FileTypes.General.Tsv)
    ];


    public AnalysisModelValidator() : base([AnalysisType.RNASeqSc, AnalysisType.RNASeqSn])
    {
        foreach (var resource in _requiredResources)
        {
            RuleFor(model => model.Resources)
                .Must(resources => HasRequiredResource(resources, resource.Name, resource.Format))
                .When(model => model.Resources?.Length > 0)
                .WithMessage($"Should contain exactly one '{resource.Name}' resource in '{resource.Format}' format");
        }
    }


    private static bool HasRequiredResource(ResourceModel[] resources, string name, string format)
    {
        var matches = resources.Where(resource => resource?.Name == name).ToArray();
        return matches.Length == 1 && matches[0].Format == format;
    }
}
