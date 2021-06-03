using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class MutationsModelValidator : AbstractValidator<MutationsModel>
    {
        private readonly IValidator<AnalysisModel> _analysisModelValidator;
        private readonly IValidator<AnalysedSampleModel> _analysedSampleModelValidator;

        public MutationsModelValidator()
        {
            _analysisModelValidator = new AnalysisModelValidator();
            _analysedSampleModelValidator = new AnalysedSampleModelValidator();


            RuleFor(model => model.Analysis)
                .SetValidator(_analysisModelValidator)
                .When(model => model.Analysis != null);


            RuleFor(model => model.Samples)
                .NotEmpty()
                .WithMessage("Should not be empty");

            RuleForEach(model => model.Samples)
                .SetValidator(_analysedSampleModelValidator);


            RuleFor(model => model)
                .Must(EachSampleNameIsUnique)
                .When(model => model.Samples != null)
                .WithMessage("Each sample name should be unique");

            RuleFor(model => model)
                .Must(EachMatchedSampleNameMatchesSingleAnalysedSampleName)
                .When(model => model.Samples != null)
                .WithMessage("Each matched sample name should match single analysed sample name");
        }


        private bool EachSampleNameIsUnique(MutationsModel model)
        {
            var names = model.Samples.Select(sample => sample.Name.Trim());
            var allNamesNumber = names.Count();
            var uniqueNamesNumber = names.Distinct().Count();

            return allNamesNumber == uniqueNamesNumber;
        }

        private bool EachMatchedSampleNameMatchesSingleAnalysedSampleName(MutationsModel model)
        {
            foreach(var sample in model.Samples)
            {
                if(sample.MatchedSamples != null)
                {
                    foreach(var matchedSampleName in sample.MatchedSamples)
                    {
                        var samples = model.Samples
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


    public class MutationsModelsValidator : AbstractValidator<IEnumerable<MutationsModel>>
    {
        private readonly IValidator<MutationsModel> _mutationsModelValidator;

        public MutationsModelsValidator()
        {
            _mutationsModelValidator = new MutationsModelValidator();

            RuleForEach(model => model)
                .SetValidator(_mutationsModelValidator);
        }
    }
}
