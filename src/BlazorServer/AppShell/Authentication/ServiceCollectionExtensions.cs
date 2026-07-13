namespace BlazorServer.AppShell.Authentication;

public static class ServiceCollectionExtensions
{
    private static Type? loginComponent = null;
    public static IServiceCollection SetLoginDisplay<T>(this IServiceCollection services)
        where T : Microsoft.AspNetCore.Components.ComponentBase
    {
        if(loginComponent == null) {
            loginComponent = typeof(T);
            services.AddSingleton(LoginDisplayRecord.Create<T>());
            return services;
        }
        throw new InvalidOperationException(
                $"Login display component is already registered as {loginComponent.FullName}.");
    }
}
