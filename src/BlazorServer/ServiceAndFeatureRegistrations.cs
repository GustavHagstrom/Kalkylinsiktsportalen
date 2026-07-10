using BlazorServer.Shared.Constants;
using BlazorServer.AppShell.Settings;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Bidcon;
using BlazorServer.Modules.Projects;
using BlazorServer.Shared.Components.Authentication;
using BlazorServer.AppShell.Composition;
using BlazorServer.Shared.Services;
using BlazorServer.Shared.Services.Authentication;
using BlazorServer.Shared.Stores.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Kiota.Abstractions.Authentication;
using MudBlazor.Services;
using BlazorServer.Modules.Reports;

namespace BlazorServer;

public static class ServiceAndFeatureRegistrations
{
    public static WebApplicationBuilder AddKipServicesAndFeatures(this WebApplicationBuilder builder)
    {

        var registrationContext = builder.Services.AddAppRegistrationContext();

        
        builder.Services.AddSettingsShell(registrationContext);
        builder.Services.AddAccessModule(registrationContext);
        builder.Services.AddBidconModule(
            registrationContext,
            builder.Configuration["Bidcon:BaseUrl"] ??
            throw new InvalidOperationException("Bidcon:BaseUrl configuration is missing"));
        builder.Services.AddProjectsModule(registrationContext);
        builder.Services.AddReportsModule(registrationContext);


        if (builder.Configuration["Auth:Mode"] == "Mock")
        {
            registrationContext.Ui.SetLoginDisplay<MockedLoginDisplay>();
            builder.Services.AddScoped<IAuthService, MockedAuthService>();
            builder.Services.AddSingleton<MockedAuthSession>();
            builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthConstants.MockAuthScheme, null);
            builder.Services.AddTransient<IUserService, MockedUserService>();
            builder.Services.AddTransient<IRoleService, MockedRoleService>();
            builder.Services.AddMemoryCache();
        }
        else if (builder.Configuration["Auth:Mode"] == "EntraId")
        {
            registrationContext.Ui.SetLoginDisplay<LoginDisplay>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(options =>
                    {
                        var section = builder.Configuration.GetSection("AzureAd");
                        section.Bind(options);
                        options.Scope.Add("email");
                    })
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddInMemoryTokenCaches();
            builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
            builder.Services.AddTransient<IAuthenticationProvider, GraphClientAuthProvider>();
            builder.Services.AddScoped(sp => new GraphServiceClient(sp.GetRequiredService<IAuthenticationProvider>()));
        }

        builder.UseMongoDb();
        builder.Services.AddSingleton<IAdminRoleCache, MemoryAdminRoleCache>();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddLocalization();
        builder.Services.AddMudServices();
        builder.Services.ApplyAppRegistrationContext(registrationContext);
        return builder;
    }

}
