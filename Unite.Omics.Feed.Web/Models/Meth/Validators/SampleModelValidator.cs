using FluentValidation;
using Unite.Data.Constants;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Models.Meth.Validators;

public class SampleModelValidator : Base.Validators.SampleModelValidator
{
    private const string RedSuffix = "_red.idat";
    private const string GreenSuffix = "_grn.idat";


    public SampleModelValidator() : base([AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS])
    {
        RuleFor(model => model.Resources)
            .Must(HaveCompleteIdatPair)
            .When(model => model.AssayType == AnalysisType.MethArray)
            .WithMessage("IDAT resources must contain one matching '*_Red.idat' and '*_Grn.idat' pair");
    }


    private static bool HaveCompleteIdatPair(ResourceModel[] resources)
    {
        var idats = resources?.Where(resource => resource?.Format == FileTypes.Sequence.Idat).ToArray() ?? [];
        if (idats.Length == 0)
            return true;

        if (idats.Length != 2)
            return false;

        var red = idats.FirstOrDefault(resource => resource.Name?.EndsWith(RedSuffix, StringComparison.Ordinal) == true);
        var green = idats.FirstOrDefault(resource => resource.Name?.EndsWith(GreenSuffix, StringComparison.Ordinal) == true);

        return red != null && green != null && red.Name.Length > RedSuffix.Length &&
            red.Name[..^RedSuffix.Length] == green.Name[..^GreenSuffix.Length];
    }
}
