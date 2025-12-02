namespace BlazorServer.Shared.Stores.MongoDb.Enteties;

public class MongoUserAccess
{
    public string UserId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public List<string> AccessClaimValues { get; set; } = []; 
}
