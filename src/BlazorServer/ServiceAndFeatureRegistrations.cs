using BlazorServer.Shared.Components.Authentication;
using BlazorServer.Shared.FeatureRegistration;
using BlazorServer.Shared.Services;
using BlazorServer.Shared.Services.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

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
}
