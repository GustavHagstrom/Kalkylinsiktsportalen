namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot.ExternalDTOs;

public class ApiKeyAuthenticateResultDto
{
    public string AccessToken { get; set; } = string.Empty;

    public int ExpireInSeconds { get; set; }
}
