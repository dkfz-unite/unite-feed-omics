using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class MutationValidator : AbstractValidator<Mutation>
    {
        private readonly IValidator<Gene> _geneValidator;

        public MutationValidator()
        {
            _geneValidator = new GeneValidator();

            RuleFor(mutation => mutation.Id)
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(mutation => mutation.Chromosome)
                .NotEmpty().When(mutation => string.IsNullOrWhiteSpace(mutation.Contig)).WithMessage("Should not be empty if 'Contig' is not set");

            RuleFor(mutation => mutation.Chromosome)
               .Empty().When(mutation => !string.IsNullOrWhiteSpace(mutation.Contig)).WithMessage("Should be empty if 'Contig' is set");

            RuleFor(mutation => mutation.Contig)
                .NotEmpty().When(mutation => !mutation.Chromosome.HasValue).WithMessage("Should not be empty if 'Chromosome' is not set")
                .MaximumLength(50).WithMessage("Maximum length is 50");

            RuleFor(mutation => mutation.Contig)
                .Empty().When(mutation => mutation.Chromosome.HasValue).WithMessage("Should be empty if 'Chromosome' is set");

            RuleFor(mutation => mutation.SequenceType)
                .NotEmpty().WithMessage("Should not be empty");

            RuleFor(mutation => mutation.Position)
                .NotEmpty().WithMessage("Should not be empty");

            RuleFor(mutation => mutation.Type)
                .NotEmpty().WithMessage("Should not be empty");

            RuleFor(mutation => mutation.Ref)
                .NotEmpty().When(mutation => string.IsNullOrWhiteSpace(mutation.Alt)).WithMessage("At least one of 'Ref' or 'Alt' fields has to be set")
                .MaximumLength(200).WithMessage("Maximum length is 200");

            RuleFor(mutation => mutation.Alt)
                .NotEmpty().When(mutation => string.IsNullOrWhiteSpace(mutation.Ref)).WithMessage("At least one of 'Ref' or 'Alt' fields has to be set")
                .MaximumLength(200).WithMessage("Maximum length is 200");

            RuleFor(mutation => mutation.Gene)
                .SetValidator(_geneValidator).When(mutation => mutation.Gene != null);
        }
    }
}
