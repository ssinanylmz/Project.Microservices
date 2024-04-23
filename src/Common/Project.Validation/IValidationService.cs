namespace Project.Validation
{
    public interface IValidationService
    {
        ValidationResponse? Validate(Type type, dynamic request);
    }

    public class ValidationResponse
    {
        public ValidationResponse() => ErrorMessages = new List<string>();

        public bool IsValid { get; set; }

        public string? ErrorMessage { get; set; }

        public List<string> ErrorMessages { get; }
    }
}
