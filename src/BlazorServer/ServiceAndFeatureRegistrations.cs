using BlazorServer.Features.Access;
using BlazorServer.Features.BidconAccess;
using BlazorServer.Features.Settings;
using BlazorServer.Shared.Components.Authentication;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Registration;
using BlazorServer.Shared.Services;
using BlazorServer.Shared.Services.Authentication;
using BlazorServer.Shared.Services.Bidcon;
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
        
        if (builder.Environment.IsDevelopment())
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
        else
        {
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(options =>
                    {
                        var section = builder.Configuration.GetSection("AzureAd");
                        section.Bind(options);
                        options.Events.OnTokenValidated += OnTokenValidatedFunc;
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

            if (builder.Environment.IsDevelopment())
            {
                config.RenderCollection.SetLoginDisplayComponent<MockedLoginDisplay>();
            }
            else
            {
                config.RenderCollection.SetLoginDisplayComponent<LoginDisplay>();
            }
        });
        builder.Services.AddHttpClient(HttpClientNames.BidconApiClient, client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Bidcon:BaseUrl"] ?? throw new InvalidOperationException("BidconApi:BaseUrl configuration is missing"));
        });

        builder.UseMongoDb();
        builder.Services.AddTransient<IBidconAccess, ApiAccess>();
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
        var featureRegistrator = new FeatureRegistrator(builder);
        config(featureRegistrator);
        builder.Services.AddSingleton(featureRegistrator.RenderCollection);
        builder.Services.AddSingleton(featureRegistrator.ClaimInfoCollection);
        builder.Services.AddAuthorization(options =>
        {
            foreach (var policy in featureRegistrator.Policies.PolicyList)
            {
                options.AddPolicy(policy.Name, policy.Builder);
            }
        });
    }


    static async Task OnTokenValidatedFunc(TokenValidatedContext context)
    {
        //implement access claims retrieval here

        //var userId = context.Principal?.GetUserId();
        //var orgId = context.Principal?.GetOrganizationId();
        //if (userId is null || orgId is null) return;

        //var roleService = context.HttpContext.RequestServices.GetRequiredService<IRoleService>();
        //if (roleService is null) return;

        //var isAdmin = await roleService.IsAdminAsync(userId, orgId);
        //if (isAdmin)
        //{
        //    ((ClaimsIdentity)context.Principal?.Identity!).AddClaim(new Claim(ClaimTypes.Role, AuthConstants.AdminRoleClaimValue));
        //}

    }

}

