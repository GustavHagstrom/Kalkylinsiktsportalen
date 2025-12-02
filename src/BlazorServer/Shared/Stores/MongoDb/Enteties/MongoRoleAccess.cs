namespace BlazorServer.Shared.Stores.MongoDb.Enteties;

public class MongoRoleAccess
{
    public string RoleId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public List<string> AccessClaimValues { get; set; } = []; 
}
