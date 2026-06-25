namespace BlazorServer.Modules.Projects.Domain;

public sealed class ProjectDomainException : InvalidOperationException
{
    public ProjectDomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
