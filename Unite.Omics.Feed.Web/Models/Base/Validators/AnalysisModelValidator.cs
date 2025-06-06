using FluentValidation;
using Unite.Data.Constants;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class AnalysisModelValidator<T, TValidator> : AbstractValidator<AnalysisModel<T>>
    where T : class, new()
    where TValidator : IValidator<T>, new()
{
    private readonly IValidator<SampleModel> _sampleModelValidator;
    private readonly IValidator<ResourceModel> _resourceModelValidator;
    private readonly IValidator<T> _entryModelValidator;


    public AnalysisModelValidator()
    {
        _sampleModelValidator = new SampleModelValidator();
        _resourceModelValidator = new ResourceModelValidator();
        _entryModelValidator = new TValidator();

        RuleFor(model => model.TargetSample)
            .NotEmpty().WithMessage("Should not be empty")
            .SetValidator(_sampleModelValidator);

        RuleFor(model => model.MatchedSample)
            .SetValidator(_sampleModelValidator)
            .When(model => model.MatchedSample != null);


        RuleFor(model => model.Resources)
            .NotEmpty().WithMessage("Should not be empty if `entries` are not set")
            .When(model => model.Entries.IsEmpty());

        RuleForEach(model => model.Resources)
            .SetValidator(_resourceModelValidator)
            .When(model => model.Resources.IsNotEmpty());


        RuleFor(model => model.Entries)
            .NotEmpty().WithMessage("Should not be empty if `resources` are not set")
            .When(model => model.Resources.IsEmpty());

        RuleForEach(model => model.Entries)
            .SetValidator(_entryModelValidator)
            .When(model => model.Entries.IsNotEmpty());


        RuleFor(model => model)
            .Must(model => HaveValidGenome(model.TargetSample))
            .WithMessage($"Only '{GenomeOptions.Build}' reference genome is supported for DNA and bulk RNA samples")
            .When(model => model.TargetSample != null && model.TargetSample.Genome != null);

        RuleFor(model => model)
            .Must(model => HaveValidGenome(model.MatchedSample))
            .WithMessage($"Only '{GenomeOptions.Build}' reference genome is supported for DNA and bulk RNA samples")
            .When(model => model.MatchedSample != null && model.MatchedSample.Genome != null);

        var rnascTypes = new AnalysisType?[] { AnalysisType.RNASeqSc, AnalysisType.RNASeqSn};
        RuleFor(model => model)
            .Must(model => HaveValidMtxResources(model.Resources))
            .When(model => model.TargetSample != null && rnascTypes.Contains(model.TargetSample.AnalysisType))
            .WithMessage("Should have 'matrix', 'barcodes', and 'features' mtx resources");
    }


    private static bool HaveValidGenome(SampleModel model)
    {
        var analyses = new AnalysisType?[] { AnalysisType.WGS, AnalysisType.WES, AnalysisType.RNASeq };

        if (analyses.Contains(model.AnalysisType))
            return string.Equals(model.Genome, GenomeOptions.Build, StringComparison.InvariantCultureIgnoreCase);
        else
            return true;
    }

    private static bool HaveValidMtxResources(ResourceModel[] resources)
    {
        var comparison = StringComparison.InvariantCultureIgnoreCase;

        var filtered = resources.Where(resource => resource.Type == DataTypes.Omics.Rnasc.Exp).ToArray();
        var mtx = filtered.FirstOrDefault(resource => resource.Format == FileTypes.Sequence.Mtx && resource.Name?.Contains("matrix", comparison) == true);
        var barcodes = filtered.FirstOrDefault(resource => resource.Format == FileTypes.General.Tsv && resource.Name?.Contains("barcodes", comparison) == true);
        var features = filtered.FirstOrDefault(resource => resource.Format == FileTypes.General.Tsv && resource.Name?.Contains("features", comparison) == true);
        if (mtx == null && barcodes == null && features == null)
            return false;

        return true;
    }
}
