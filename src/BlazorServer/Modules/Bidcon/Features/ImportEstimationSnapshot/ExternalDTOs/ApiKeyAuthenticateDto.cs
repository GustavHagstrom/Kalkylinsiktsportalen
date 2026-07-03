namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot.ExternalDTOs;

public class ApiKeyAuthenticateDto
{        
    public string ApiKey { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
}
