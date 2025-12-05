namespace BlazorServer.Shared.Constants;

public static class AuthConstants
{
    public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
    public const string AdminRoleClaimValue = "Admin";
    public const string RequireAuthenticatedUserPolicyName = "RequireAuthentication";
}
