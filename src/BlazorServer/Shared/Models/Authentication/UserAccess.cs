namespace BlazorServer.Shared.Models;
public class UserAccess
{
    public string UserId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public List<string> AccessClaimValues { get; set; } = [];
}
