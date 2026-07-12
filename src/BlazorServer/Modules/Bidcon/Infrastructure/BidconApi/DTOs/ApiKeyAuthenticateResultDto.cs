namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

internal class ApiKeyAuthenticateResultDto
{
    public string AccessToken { get; set; } = string.Empty;

    public int ExpireInSeconds { get; set; }
}
