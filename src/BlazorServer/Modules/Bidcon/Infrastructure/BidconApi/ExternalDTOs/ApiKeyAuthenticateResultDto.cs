namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

public class ApiKeyAuthenticateResultDto
{
    public string AccessToken { get; set; } = string.Empty;

    public int ExpireInSeconds { get; set; }
}
