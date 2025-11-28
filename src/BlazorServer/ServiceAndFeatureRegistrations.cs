using BlazorServer.Shared.Components.Authentication;
using BlazorServer.Shared.FeatureRegistration;
using BlazorServer.Shared.Services;
using BlazorServer.Shared.Services.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Kiota.Abstractions.Authentication;
using MudBlazor.Services;
using System.Security.Claims;

namespace BlazorServer;

public static class ServiceAndFeatureRegistrations
{
    public static WebApplicationBuilder AddKipServicesAndFeatures(this WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
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
            if (builder.Environment.IsDevelopment())
            {
                config.RenderCollection.SetLoginDisplayComponent<MockedLoginDisplay>();
            }
            else
            {
                config.RenderCollection.SetLoginDisplayComponent<LoginDisplay>();
            }

                
        });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddLocalization();
        builder.Services.AddMudServices();
        return builder;
    }
    public static void ConfigureFeatures(this WebApplicationBuilder builder, Action<FeatureBuilder> config)
    {
        var sp = builder.Services.BuildServiceProvider();
        var featureBuilder = new FeatureBuilder(builder, sp);
        config(featureBuilder);
        builder.Services.AddSingleton(featureBuilder.RenderCollection);

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

