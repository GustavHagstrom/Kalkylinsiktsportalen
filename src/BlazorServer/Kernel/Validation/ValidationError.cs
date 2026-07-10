namespace BlazorServer.Kernel.Validation;

public sealed record ValidationError(string Code, string Field, string Message);
