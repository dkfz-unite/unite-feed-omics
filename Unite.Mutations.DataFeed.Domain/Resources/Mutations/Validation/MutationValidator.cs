using System.Collections.Generic;
using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class MutationValidator : AbstractValidator<Mutation>
    {
        private readonly IValidator<Gene> _geneValidator;
        private readonly IValidator<Sample> _sampleValidator;

        public MutationValidator()
        {
            _geneValidator = new GeneValidator();
            _sampleValidator = new SampleValidator();

            RuleFor(mutation => mutation.Pid)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(mutation => mutation.Id)
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(mutation => mutation.Chromosome)
                .NotEmpty().When(mutation => string.IsNullOrWhiteSpace(mutation.Contig)).WithMessage("Should not be empty if 'Contig' is not set");

            RuleFor(mutation => mutation.Chromosome)
               .Empty().When(mutation => !string.IsNullOrWhiteSpace(mutation.Contig)).WithMessage("Should be empty if 'Contig' is set");

            RuleFor(mutation => mutation.Contig)
                .NotEmpty().When(mutation => !mutation.Chromosome.HasValue).WithMessage("Should not be empty if 'Chromosome' is not set")
                .MaximumLength(500).WithMessage("Maximum length is 50");

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

            RuleFor(mutation => mutation.Quality)
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(mutation => mutation.Filter)
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(mutation => mutation.Gene)
                .SetValidator(_geneValidator).When(mutation => mutation.Gene != null);

            RuleFor(mutation => mutation.Sample)
                .SetValidator(_sampleValidator).When(mutation => mutation.Sample != null);
        }
    }

    public class MutationsValidator : AbstractValidator<IEnumerable<Mutation>>
    {
        private readonly IValidator<Mutation> _mutationValidator;

        public MutationsValidator()
        {
            _mutationValidator = new MutationValidator();

            RuleForEach(mutations => mutations)
                .SetValidator(_mutationValidator).When(mutations => mutations != null);
        }
    }
}
