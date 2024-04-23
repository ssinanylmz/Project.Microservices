using FluentValidation;
using System.Text;

namespace Project.Validation
{
    public class ValidationService : IValidationService
    {
        public ValidationResponse? Validate(Type type, dynamic request)
        {
            if (!(Activator.CreateInstance(type) is IValidator validator)) return null;

            var context = new ValidationContext<object>(request);

            var result = validator.Validate(context);

            var errorMessages = new StringBuilder();

            foreach (var item in result.Errors)
                errorMessages.AppendLine(item.ErrorMessage);

            var response = new ValidationResponse
            {
                IsValid = result.IsValid,
                ErrorMessage = result.IsValid
                    ? string.Empty
                    : result.Errors[0]?.ToString()
            };

            foreach (var item in result.Errors)
                response.ErrorMessages.Add(item.ErrorMessage);

            return response;
        }
    }
}
