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
                .Must(EachSampleIdIsUnique)
                .When(model => model.Samples != null)
                .WithMessage("Each sample id should be unique");

            RuleFor(model => model)
                .Must(EachMatchedSampleIdMatchesSingleAnalysedSampleId)
                .When(model => model.Samples != null)
                .WithMessage("Each matched sample id should match single analysed sample id");
        }


        private bool EachSampleIdIsUnique(MutationsModel model)
        {
            var ids = model.Samples.Select(sample => sample.Id.Trim());
            var allIdsNumber = ids.Count();
            var uniqueIdsNumber = ids.Distinct().Count();

            return allIdsNumber == uniqueIdsNumber;
        }

        private bool EachMatchedSampleIdMatchesSingleAnalysedSampleId(MutationsModel model)
        {
            foreach (var analysedSample in model.Samples)
            {
                if (!string.IsNullOrWhiteSpace(analysedSample.MatchedSampleId))
                {
                    var samples = model.Samples
                        .Where(sample => string.Equals(sample.Id.Trim(), analysedSample.MatchedSampleId.Trim()))
                        .ToArray();

                    if (samples.Length != 1)
                    {
                        return false;
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
