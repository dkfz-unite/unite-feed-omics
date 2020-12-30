using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Validation
{
    public interface IValidationService
    {
        bool ValidateParameter<T>(T parameter, IValidator<T> validator, out string errorMessage);
    }
}
