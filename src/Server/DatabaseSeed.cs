namespace Server;

public static class DatabaseSeed
{
    public static void EnsureSeedData(WebApplication app)
    {
        //Should be logged
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            SeedLicenseDb(scope);
        }
    }
    private static void SeedLicenseDb(IServiceScope scope)
    {
        //Should be logged

        //var licenseClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
        //var seedModel = new AppSeedModel
        //{
        //    ApplicationName = CommonAppConstants.ApplicationName,
        //    Roles = CommonAppConstants.AppRoles,
        //};
        //var result = licenseClient.PostAsJsonAsync(LicenseApiEndpoints.ApplicationSeed, seedModel).Result;
    }
}
