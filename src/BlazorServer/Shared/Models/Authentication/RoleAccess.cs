namespace BlazorServer.Shared.Models;
public class RoleAccess
{
    public string RoleId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public List<string> AccessClaimValues { get; set; } = [];
}
