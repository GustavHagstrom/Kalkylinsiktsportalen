using BlazorServer.Identity;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public sealed class BidconApiCredentials
{
    public OrganizationId OrganizationId { get; set; }

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;
}
