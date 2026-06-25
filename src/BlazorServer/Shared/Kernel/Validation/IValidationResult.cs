namespace BlazorServer.Shared.Kernel.Validation;

public interface IValidationResult
{
    bool IsValid { get; }

    IReadOnlyList<ValidationError> Errors { get; }
}
