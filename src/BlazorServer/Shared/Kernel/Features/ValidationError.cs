namespace BlazorServer.Shared.Kernel.Features;

public sealed record ValidationError(string Code, string Field, string Message);
