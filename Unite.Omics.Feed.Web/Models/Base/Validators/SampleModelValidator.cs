﻿using FluentValidation;
using Unite.Data.Constants;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

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


        var methArrayTypes = new AnalysisType?[] { AnalysisType.MethArray };
        RuleFor(model => model.Resources)
            .Must(HasValidIdatResources)
            .When(model => methArrayTypes.Contains(model.AnalysisType) && model.Resources.IsNotEmpty())
            .WithMessage("Should have 'red' and 'grn' idat resources");
    }

    private static bool HasValidIdatResources(ResourceModel[] resources)
    {
        var comparison = StringComparison.InvariantCultureIgnoreCase;

        var filtered = resources.Where(resource => resource.Type == DataTypes.Omics.Meth.Sample).ToArray();
        var red = filtered.FirstOrDefault(resource => resource.Format == FileTypes.Sequence.Idat && resource.Name?.Contains("red", comparison) == true);
        var green = filtered.FirstOrDefault(resource => resource.Format == FileTypes.Sequence.Idat && resource.Name?.Contains("grn", comparison) == true);
        if (red == null && green == null)
            return false;

        return true;
    }
}
