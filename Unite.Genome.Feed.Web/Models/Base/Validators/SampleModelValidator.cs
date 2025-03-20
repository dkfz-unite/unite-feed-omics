using FluentValidation;
using Unite.Data.Constants;
using Unite.Data.Entities.Genome.Analysis.Enums;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class SampleModelValidator : AbstractValidator<SampleModel> 
{
    private readonly IValidator<ResourceModel> _resourceModelValidator = new ResourceModelValidator();

    public SampleModelValidator()
    {
        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.DonorId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");


        RuleFor(model => model.SpecimenId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.SpecimenId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");


        RuleFor(model => model.SpecimenType)
            .NotEmpty()
            .WithMessage("Should not be empty");

        
        RuleFor(model => model.AnalysisType)
            .NotEmpty()
            .WithMessage("Should not be empty");


        RuleFor(model => model.AnalysisDate)
            .Empty()
            .When(model => model.AnalysisDay != null)
            .WithMessage("Either exact 'analysis_date' or relative 'analysis_day' can be set, not both");


        RuleFor(model => model.AnalysisDay)
            .Empty()
            .When(model => model.AnalysisDate != null)
            .WithMessage("Either exact 'analysis_date' or relative 'analysis_day' can be set, not both");

        RuleFor(model => model.AnalysisDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.AnalysisDay != null)
            .WithMessage("Should be greater than or equal to 1");


        RuleFor(model => model.Genome)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        
        RuleFor(model => model.Purity)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1)
            .WithMessage("Should be in range [0, 1]");

        
        RuleFor(model => model.Ploidy)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Should be greater than or equal to 1");


        RuleFor(model => model.Cells)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Should be greater than or equal to 1");


        RuleForEach(model => model.Resources)
            .SetValidator(_resourceModelValidator);


        var methArrayTypes = new AnalysisType[] { AnalysisType.MethArray };
        RuleFor(model => model.Resources)
            .Must(HasValidIdatResources)
            .When(model => methArrayTypes.Contains(model.AnalysisType.Value))
            .WithMessage("Should have 'red' and 'green' idat resources");


        var rnascTypes = new AnalysisType[] { AnalysisType.RNASeqSc, AnalysisType.RNASeqSn};
        RuleFor(model => model.Resources)
            .Must(HasValidMtxResources)
            .When(model => rnascTypes.Contains(model.AnalysisType.Value))
            .WithMessage("Should have 'matrix', 'barcodes', and 'features' mtx resources");
            
    }

    private static bool HasValidIdatResources(ResourceModel[] resources)
    {
        var comparison = StringComparison.InvariantCultureIgnoreCase;

        var filtered = resources.Where(resource => resource.Type == DataTypes.Genome.Meth.Sample).ToArray();
        if (filtered.Length != 2)
            return false;

        var red = filtered.FirstOrDefault(resource => resource.Format == FileTypes.Sequence.Idat && resource.Name.Equals("red", comparison));
        var green = filtered.FirstOrDefault(resource => resource.Format == FileTypes.Sequence.Idat && resource.Name.Equals("green", comparison));
        if (red == null || green == null)
            return false;

        return true;
    }

    private static bool HasValidMtxResources(ResourceModel[] resources)
    {
        var comparison = StringComparison.InvariantCultureIgnoreCase;

        var filtered = resources.Where(resource => resource.Type == DataTypes.Genome.Rnasc.Exp).ToArray();
        if (filtered.Length != 3)
            return false;

        var mtx = filtered.FirstOrDefault(resource => resource.Format == FileTypes.Sequence.Mtx);
        var barcodes = filtered.FirstOrDefault(resource => resource.Format == FileTypes.General.Tsv && resource.Name.Equals("barcodes", comparison));
        var features = filtered.FirstOrDefault(resource => resource.Format == FileTypes.General.Tsv && resource.Name.Equals("features", comparison));
        if (mtx == null || barcodes == null || features == null)
            return false;

        return true;
    }
}
