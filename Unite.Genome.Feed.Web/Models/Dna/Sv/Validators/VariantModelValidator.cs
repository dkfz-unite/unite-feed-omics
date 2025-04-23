using FluentValidation;
using Unite.Data.Entities.Genome.Analysis.Dna.Sv.Enums;

namespace Unite.Genome.Feed.Web.Models.Dna.Sv.Validators;

public class VariantModelValidator : AbstractValidator<VariantModel>
{
    public VariantModelValidator()
    {
        RuleFor(model => model.Chromosome)
            .NotEmpty().WithMessage("Should not be empty");

        RuleFor(model => model.Start)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.End)
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.OtherChromosome)
            .NotEmpty().WithMessage("Should not be empty");

        RuleFor(model => model.OtherStart)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.OtherEnd)
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model)
            .Must(model => model.OtherStart - model.End > 0)
            .When(model => model.Type != SvType.ITX && model.Type != SvType.CTX)
            .WithMessage("'start_2' should be greater than 'end_1'");

        RuleFor(model => model.Type)
            .NotEmpty().WithMessage("Should not be empty");

        RuleFor(model => model.FlankingSequenceFrom)
            .MaximumLength(200).WithMessage("Maximum length is 200");

        RuleFor(model => model.FlankingSequenceTo)
            .MaximumLength(200).WithMessage("Maximum length is 200");
    }
}
