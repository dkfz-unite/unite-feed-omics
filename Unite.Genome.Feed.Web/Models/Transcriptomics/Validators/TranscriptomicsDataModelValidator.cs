using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Transcriptomics.Validators;

public class TranscriptomicsDataModelValidator : AbstractValidator<TranscriptomicsDataModel>
{
    private readonly IValidator<AnalysisModel> _analysisModelValidator;
    private readonly IValidator<AnalysedSampleModel> _analysedSampleModelValidator;
    private readonly IValidator<ExpressionModel> _expressionModelValidator;

    public TranscriptomicsDataModelValidator()
    {
        _analysisModelValidator = new AnalysisModelValidator();
        _analysedSampleModelValidator = new AnalysedSampleModelValidator();
        _expressionModelValidator = new ExpressionModelValidator();


        RuleFor(model => model.Analysis)
            .SetValidator(_analysisModelValidator);


        RuleFor(model => model.Sample)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Sample)
            .SetValidator(_analysedSampleModelValidator);


        RuleFor(model => model.Expressions)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleForEach(model => model.Expressions)
            .SetValidator(_expressionModelValidator);
    }
}


public class ExpressionDataModelsValidator : AbstractValidator<IEnumerable<TranscriptomicsDataModel>>
{
    private readonly IValidator<TranscriptomicsDataModel> _expressionDataModelValidator;

    public ExpressionDataModelsValidator()
    {
        _expressionDataModelValidator = new TranscriptomicsDataModelValidator();

        RuleForEach(model => model)
            .SetValidator(_expressionDataModelValidator);
    }
}
