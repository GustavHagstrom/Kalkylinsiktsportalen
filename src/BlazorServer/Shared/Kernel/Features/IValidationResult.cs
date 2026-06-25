namespace BlazorServer.Shared.Kernel.Features;

public interface IValidationResult
{
    bool IsValid { get; }

    IReadOnlyList<ValidationError> Errors { get; }
}
