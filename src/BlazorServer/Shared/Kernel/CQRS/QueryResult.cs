using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Shared.Kernel.CQRS;

public sealed class QueryResult<T> : IValidationResult
{
    private QueryResult(T? value, IReadOnlyList<ValidationError> errors)
    {
        Value = value;
        Errors = errors;
    }

    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public T? Value { get; }

    public IReadOnlyList<ValidationError> Errors { get; }

    public static QueryResult<T> Success(T value) => new(value, Array.Empty<ValidationError>());

    public static QueryResult<T> Invalid(params ValidationError[] errors) => new(default, errors);
}
