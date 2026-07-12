namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

internal class ApiKeyAuthenticateDto
{        
    public string ApiKey { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
}
