using FluentValidation;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class AnalysedSampleModelValidator : AbstractValidator<AnalysedSampleModel>
    {
        private readonly IValidator<TissueModel> _tissueModelValidator;
        private readonly IValidator<CellLineModel> _cellLineModelValidator;
        private readonly IValidator<XenograftModel> _xenograftModelValidator;
        private readonly IValidator<MutationModel> _mutationModelValidator;

        public AnalysedSampleModelValidator()
        {
            _tissueModelValidator = new TissueModelValidator();
            _cellLineModelValidator = new CellLineModelValidator();
            _xenograftModelValidator = new XenograftModelValidator();
            _mutationModelValidator = new MutationModelValidator();


            RuleFor(model => model.Id)
                .MaximumLength(255)
                .WithMessage("Maximum length is 255");

            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Should not be empty");


            RuleFor(model => model)
                .Must(HaveSpecimenDataSet)
                .WithMessage("Specimen data (Tissue, CellLine or Xenograft) has to be set");

            RuleFor(model => model.Tissue)
                .SetValidator(_tissueModelValidator);

            RuleFor(model => model.CellLine)
                .SetValidator(_cellLineModelValidator);

            RuleFor(model => model.Xenograft)
                .SetValidator(_xenograftModelValidator);


            RuleForEach(model => model.Mutations)
                .SetValidator(_mutationModelValidator);
        }


        private bool HaveSpecimenDataSet(AnalysedSampleModel model)
        {
            return model.Tissue != null
                || model.CellLine != null
                || model.Xenograft != null;
        }
    }
}
