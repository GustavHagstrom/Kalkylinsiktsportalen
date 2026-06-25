namespace BlazorServer.Shared.Kernel.Features;

public sealed class ValidationResult : IValidationResult
{
    private ValidationResult(IReadOnlyList<ValidationError> errors)
    {
        Errors = errors;
    }

    public bool IsValid => Errors.Count == 0;

    public IReadOnlyList<ValidationError> Errors { get; }

    public static ValidationResult Valid() => new(Array.Empty<ValidationError>());

    public static ValidationResult Invalid(params ValidationError[] errors) => new(errors);
}
