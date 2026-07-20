using BlazorServer.AppShell.Authentication;
using BlazorServer.AppShell.Authentication.Mock;
using BlazorServer.AppShell.Settings;
using BlazorServer.Composition;
using BlazorServer.Identity;
using BlazorServer.Infrastructure.MongoDb;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Bidcon;
using BlazorServer.Modules.Projects;
using BlazorServer.Modules.Reports;
using BlazorServer.UiKit.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Kiota.Abstractions.Authentication;
using MudBlazor.Services;

namespace BlazorServer;

public static class ServiceRegistrations
{
    public static WebApplicationBuilder AddKipServicesAndFeatures(this WebApplicationBuilder builder)
    {
        var registrationContext = builder.Services.AddAppRegistrationContext();

        builder.Services.AddSettingsShell(registrationContext);
        builder.Services.AddAccessModule(registrationContext);
        builder.Services.AddBidconModule(
            registrationContext,
            builder.Configuration["Bidcon:BaseUrl"]
                ?? throw new InvalidOperationException("Bidcon:BaseUrl configuration is missing"));
        builder.Services.AddProjectsModule(registrationContext);
        builder.Services.AddReportsModule(registrationContext);

        if (builder.Configuration["Auth:Mode"] == "Mock")
        {
            var mockOrganizationId = builder.Configuration["Auth:Mock:OrganizationId"] ?? "MockedOrganization";
            builder.Services.SetLoginDisplay<MockedLoginDisplay>();
            builder.Services.AddScoped<MockCurrentUserSession>();
            builder.Services.AddSingleton<MockAuthenticationState>();
            builder.Services.AddScoped<IAuthService>(sp => sp.GetRequiredService<MockCurrentUserSession>());
            builder.Services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<MockCurrentUserSession>());
            builder.Services.AddSingleton<ICurrentUserProfileLookup, MockProfileLookup>();
            builder.Services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthSchemes.Mock, null);
            builder.Services.AddSingleton(new MockAuthDefaults(mockOrganizationId));
            builder.Services.AddMockAccessDirectory(new OrganizationId(mockOrganizationId));
            builder.Services.AddMemoryCache();
        }
        else if (builder.Configuration["Auth:Mode"] == "EntraId")
        {
            builder.Services.SetLoginDisplay<LoginDisplay>();
            builder.Services.AddScoped<CurrentUserSession>();
            builder.Services.AddScoped<IAuthService>(sp => sp.GetRequiredService<CurrentUserSession>());
            builder.Services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<CurrentUserSession>());
            builder.Services.AddTransient<ICurrentUserProfileLookup, GraphCurrentUserProfileLookup>();
            builder.Services.AddGraphAccessDirectory();
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
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton<IComponentHtmlRenderer, ComponentHtmlRenderer>();
        builder.Services.AddLocalization();
        builder.Services.AddMudServices();
        builder.Services.ApplyAppRegistrationContext(registrationContext);
        return builder;
    }
}
