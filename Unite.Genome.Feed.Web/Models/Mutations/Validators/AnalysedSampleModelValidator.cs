using FluentValidation;
using Unite.Genome.Feed.Web.Services.Mutations;
using Unite.Genome.Feed.Web.Services.Mutations.Validators;

namespace Unite.Genome.Feed.Web.Services.Mutations.Validators;

public class AnalysedSampleModelValidator : AbstractValidator<AnalysedSampleModel>
{
    private readonly IValidator<MutationModel> _mutationModelValidator;

    public AnalysedSampleModelValidator()
    {
        _mutationModelValidator = new MutationModelValidator();

        RuleFor(model => model.Id)
           .NotEmpty()
           .WithMessage("Should not be empty");

        RuleFor(model => model.Id)
           .MaximumLength(255)
           .WithMessage("Maximum length is 255");


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


        RuleForEach(model => model.Mutations)
            .SetValidator(_mutationModelValidator);
    }

}
