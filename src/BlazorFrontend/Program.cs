using BlazorFrontend.Shared.Components;
using BlazorFrontend.Shared.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

if (builder.HostEnvironment.IsDevelopment())
{
    builder.Services.AddScoped<IAuthService, MockedAuthService>();
    builder.Services.AddScoped<AuthenticationStateProvider, MockedAuthenticationStateProvider>();
}
else
{
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddMsalAuthentication(options =>
    {
        builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    });

}



await builder.Build().RunAsync();

