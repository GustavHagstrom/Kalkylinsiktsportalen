namespace BlazorServer.Shared.Kernel.Validation;

public sealed record ValidationResult(IReadOnlyList<ValidationError> Errors) : IValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public static ValidationResult Success() => new(Array.Empty<ValidationError>());

    public static ValidationResult Failure(params ValidationError[] errors) => new(errors);
}
