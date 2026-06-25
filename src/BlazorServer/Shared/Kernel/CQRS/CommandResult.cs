using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Shared.Kernel.CQRS;

public sealed class CommandResult<T> : IValidationResult
{
    private CommandResult(T? value, IReadOnlyList<ValidationError> errors)
    {
        Value = value;
        Errors = errors;
    }

    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public T? Value { get; }

    public IReadOnlyList<ValidationError> Errors { get; }

    public static CommandResult<T> Success(T value) => new(value, Array.Empty<ValidationError>());

    public static CommandResult<T> Invalid(params ValidationError[] errors) => new(default, errors);
}
