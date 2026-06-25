namespace BlazorServer.Shared.Kernel.Validation;

public sealed record ValidationError(string Code, string Field, string Message);
