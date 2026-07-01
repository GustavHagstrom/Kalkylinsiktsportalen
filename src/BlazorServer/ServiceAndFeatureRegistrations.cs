using BlazorServer.Features.Access;
using BlazorServer.Features.BidconAccess;
using BlazorServer.Features.BidconBrowser;
using BlazorServer.Features.Settings;
using BlazorServer.Features.ViewSettings;
using BlazorServer.Modules.Bidcon;
using BlazorServer.Modules.Projects;
using BlazorServer.Shared.Components.Authentication;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;
using BlazorServer.Shared.ModuleRegistration;
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

namespace BlazorServer;

public static class ServiceAndFeatureRegistrations
{
    public static WebApplicationBuilder AddKipServicesAndFeatures(this WebApplicationBuilder builder)
    {
        
        if (builder.Configuration["Auth:Mode"] == "Mock")
        {
            builder.Services.AddScoped<IAuthService, MockedAuthService>();
            builder.Services.AddSingleton<MockedAuthSession>();
            builder.Services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthConstants.MockAuthScheme, null);

            //builder.Services.AddScoped<AuthenticationStateProvider, MockedAuthStateProvider>();
            builder.Services.AddTransient<IUserService, MockedUserService>();
            builder.Services.AddTransient<IRoleService, MockedRoleService>();
            builder.Services.AddMemoryCache();
        }
        else if (builder.Configuration["Auth:Mode"] == "EntraId")
        {
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

        builder.ConfigureFeatures(config =>
        {
            //config.AddAuthorizationPolicy(AuthConstants.RequireAuthenticatedUserPolicyName, policy =>
            //{
            //    policy.RequireAuthenticatedUser();
            //});
            config.AddFeature<SettingsFeature>();
            config.AddFeature<AccessFeature>();
            config.AddFeature<BidconAccessFeature>();
            config.AddFeature<BidconBrowserFeature>();
            config.AddFeature<ViewSettingsFeature>();

            if (builder.Configuration["Auth:Mode"] == "Mock")
            {
                config.RenderCollection.SetLoginDisplayComponent<MockedLoginDisplay>();
            }
            else
            {
                config.RenderCollection.SetLoginDisplayComponent<LoginDisplay>();
            }
        });

        builder.ConfigureModules(config =>
        {
            config.AddModule<BidconModule>();
            config.AddModule<ProjectsModule>();
        });

        builder.Services.AddHttpClient(HttpClientNames.BidconApiClient, client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Bidcon:BaseUrl"] ?? throw new InvalidOperationException("BidconApi:BaseUrl configuration is missing"));
        });

        builder.UseMongoDb();
        builder.Services.AddTransient<BidconApiAccess>();
        builder.Services.AddSingleton<IClaimsCache, MemoryClaimsCache>();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddLocalization();
        builder.Services.AddMudServices();
        return builder;
    }

    public static void ConfigureFeatures(this WebApplicationBuilder builder, Action<FeatureRegistrator> config)
    {
        var registrationContext = builder.GetOrCreateAppRegistrationContext();
        var featureRegistrator = new FeatureRegistrator(builder, registrationContext);
        config(featureRegistrator);
        builder.AddAppRegistrationContext(registrationContext);
    }

    public static void ConfigureModules(this WebApplicationBuilder builder, Action<ModuleRegistrator> config)
    {
        var registrationContext = builder.GetOrCreateAppRegistrationContext();
        var moduleRegistrator = new ModuleRegistrator(builder, registrationContext);
        config(moduleRegistrator);
        builder.AddAppRegistrationContext(registrationContext);
    }

    private static AppRegistrationContext GetOrCreateAppRegistrationContext(this WebApplicationBuilder builder)
    {
        var existingContext = builder.Services
            .FirstOrDefault(service => service.ServiceType == typeof(AppRegistrationContext))
            ?.ImplementationInstance as AppRegistrationContext;

        if (existingContext is not null)
        {
            return existingContext;
        }

        var registrationContext = new AppRegistrationContext();
        builder.Services.AddSingleton(registrationContext);

        return registrationContext;
    }

    private static void AddAppRegistrationContext(
        this WebApplicationBuilder builder,
        AppRegistrationContext registrationContext)
    {
        if (builder.Services.Any(service => service.ServiceType == typeof(RenderCollection)))
        {
            return;
        }

        builder.Services.AddSingleton(registrationContext.UiComposition);
        builder.Services.AddSingleton(registrationContext.RenderCollection);
        builder.Services.AddSingleton(registrationContext.ClaimInfoCollection);
        builder.Services.AddAuthorization(options =>
        {
            foreach (var policy in registrationContext.Policies.PolicyList)
            {
                options.AddPolicy(policy.Name, policy.Builder);
            }
        });
    }
}

