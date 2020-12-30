using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class GeneValidator : AbstractValidator<Gene>
    {
        public GeneValidator()
        {
            RuleFor(gene => gene.Name)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");
        }
    }
}
