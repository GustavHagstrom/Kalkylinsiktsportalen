namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public sealed class BidconApiCredentials
{
    public string OrganizationId { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;
}
