namespace BlazorServer.Identity;

public static class IdentityClaimTypes
{
    public const string UserId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    public const string OrganizationId = "http://schemas.microsoft.com/identity/claims/tenantid";
    public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    public const string Groups = "groups";
}
