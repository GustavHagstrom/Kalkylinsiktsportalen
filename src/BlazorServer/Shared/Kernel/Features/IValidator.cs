namespace BlazorServer.Shared.Kernel.Features;

public interface IValidator<in TRequest>
{
    ValidationResult Validate(TRequest request);
}
