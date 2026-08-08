namespace BlazorServer.Modules.Reports.Domain;

public sealed class ReportsDomainException : InvalidOperationException
{
    public ReportsDomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
