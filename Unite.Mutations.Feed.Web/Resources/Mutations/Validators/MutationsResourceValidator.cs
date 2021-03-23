using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Unite.Mutations.Feed.Web.Resources.Mutations.Validators
{
    public class MutationsResourceValidator : AbstractValidator<MutationsResource>
    {
        private readonly IValidator<AnalysisResource> _analysisResourceValidator;
        private readonly IValidator<SampleResource> _sampleResourceValidator;

        public MutationsResourceValidator()
        {
            _analysisResourceValidator = new AnalysisResourceValidator();
            _sampleResourceValidator = new SampleResourceValidator();

            RuleFor(resource => resource.Pid)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(resource => resource.Analysis)
                .SetValidator(_analysisResourceValidator).When(resource => resource.Analysis != null);

            RuleFor(resource => resource.Samples)
                .NotEmpty().WithMessage("Should not be empty");

            RuleForEach(resource => resource.Samples)
                .SetValidator(_sampleResourceValidator).When(resource => resource.Samples != null);

            RuleFor(resource => resource)
                .Must(EachSampleNameIsUnique)
                .When(resource => resource.Samples != null)
                .WithMessage("Each sample name should be unique");

            RuleFor(resource => resource)
                .Must(EachMatchedSampleNameMatchesSingleAnalysedSampleName)
                .When(resource => resource.Samples != null)
                .WithMessage("Each matched sample name should match single analysed sample name");
        }

        private bool EachSampleNameIsUnique(MutationsResource mutationsResource)
        {
            var names = mutationsResource.Samples.Select(sample => sample.Name.Trim());
            var allNamesNumber = names.Count();
            var uniqueNamesNumber = names.Distinct().Count();

            return allNamesNumber == uniqueNamesNumber;
        }

        private bool EachMatchedSampleNameMatchesSingleAnalysedSampleName(MutationsResource mutationsResource)
        {
            foreach(var sample in mutationsResource.Samples)
            {
                if(sample.MatchedSamples != null)
                {
                    foreach(var matchedSampleName in sample.MatchedSamples)
                    {
                        var samples = mutationsResource.Samples
                            .Where(sample => string.Equals(sample.Name.Trim(), matchedSampleName.Trim()))
                            .ToArray();

                        if(samples.Length != 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }

    public class MutationResourcesValidator : AbstractValidator<IEnumerable<MutationsResource>>
    {
        private readonly IValidator<MutationsResource> _mutationResourceValidator;

        public MutationResourcesValidator()
        {
            _mutationResourceValidator = new MutationsResourceValidator();

            RuleForEach(resource => resource)
                .SetValidator(_mutationResourceValidator);
        }
    }
}
