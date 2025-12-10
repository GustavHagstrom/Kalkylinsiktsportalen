namespace BlazorServer.Shared.DTOs.Bidcon;

public class ApiKeyAuthenticateResultDto
{
    public string AccessToken { get; set; } = string.Empty;

    public int ExpireInSeconds { get; set; }
}
