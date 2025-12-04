using BlazorServer.Features.Access;
using BlazorServer.Features.Settings;
using BlazorServer.Shared.Components.Authentication;
using BlazorServer.Shared.Registration;
using BlazorServer.Shared.Services;
using BlazorServer.Shared.Services.Authentication;
using BlazorServer.Shared.Stores.MongoDb;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
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
            builder.Services.AddScoped<AuthenticationStateProvider, MockedAuthStateProvider>();
            builder.Services.AddTransient<IUserService, MockedUserService>();
            builder.Services.AddTransient<IRoleService, MockedRoleService>();
        }
        else
        {
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddTransient<IUserService, UserService>();
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
            //config.AddAuthorizationPolicy("AdminOnly", policy =>
            //{
            //    policy.RequireClaim("role", "Admin");
            //});
            config.AddFeature<SettingsFeature>();
            config.AddFeature<AccessFeature>();

            if (builder.Environment.IsDevelopment())
            {
                config.RenderCollection.SetLoginDisplayComponent<MockedLoginDisplay>();
            }
            else
            {
                config.RenderCollection.SetLoginDisplayComponent<LoginDisplay>();
            }
        });
        builder.UseMongoDb();
        builder.Services.AddCascadingAuthenticationState();
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

        //var accessClaimService = context.HttpContext.RequestServices.GetRequiredService<IAccessClaimStore>();
        //if (accessClaimService is not null)
        //{
        //    var claims = (await accessClaimService.GetAccessClaims(context.Principal)).ToList();
        //    context.Principal?.AddIdentity(new ClaimsIdentity(claims));
        //}
        //else
        //{
        //    //handle?
        //}
        //await Task.CompletedTask.ConfigureAwait(false);
    }

}

