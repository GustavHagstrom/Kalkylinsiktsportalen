namespace BlazorServer.Shared.Kernel.Validation;

public interface IValidator<T>
{
    ValidationResult Validate(T instance);
}
