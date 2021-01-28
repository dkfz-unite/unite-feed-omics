using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class ResourceValidator : AbstractValidator<Resource>
    {
        private readonly IValidator<Analysis> _analysisValidator;
        private readonly IValidator<Sample> _sampleValidator;

        public ResourceValidator()
        {
            _analysisValidator = new AnalysisValidator();
            _sampleValidator = new SampleValidator();

            RuleFor(resource => resource.Pid)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(resource => resource.Analysis)
                .SetValidator(_analysisValidator).When(resource => resource.Analysis != null);

            RuleFor(resource => resource.Samples)
                .NotEmpty().WithMessage("Should not be empty");

            RuleForEach(resource => resource.Samples)
                .SetValidator(_sampleValidator).When(resource => resource.Samples != null);

            RuleFor(resource => resource)
                .Must(EachSampleNameIsUnique)
                .When(resource => resource.Samples != null)
                .WithMessage("Each sample name should be unique");

            RuleFor(resource => resource)
                .Must(EachMatchedSampleNameMatchesSingleAnalysedSampleName)
                .When(resource => resource.Samples != null)
                .WithMessage("Each matched sample name should match single analysed sample name");
        }

        private bool EachSampleNameIsUnique(Resource resource)
        {
            var names = resource.Samples.Select(sample => sample.Name.Trim());
            var allNamesNumber = names.Count();
            var uniqueNamesNumber = names.Distinct().Count();

            return allNamesNumber == uniqueNamesNumber;
        }

        private bool EachMatchedSampleNameMatchesSingleAnalysedSampleName(Resource resource)
        {
            foreach(var sample in resource.Samples)
            {
                if(sample.MatchedSamples != null)
                {
                    foreach(var matchedSampleName in sample.MatchedSamples)
                    {
                        var samples = resource.Samples
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

    public class ResourcesValidator : AbstractValidator<IEnumerable<Resource>>
    {
        private readonly IValidator<Resource> _resourceValidator;

        public ResourcesValidator()
        {
            _resourceValidator = new ResourceValidator();

            RuleForEach(resources => resources)
                .SetValidator(_resourceValidator).When(resources => resources != null);
        }
    }
}
