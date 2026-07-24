using System.Reflection;
using BlazorServer;

namespace BlazorServer.Tests.AppShell;

[TestFixture]
public class UiOwnershipTests
{
    private static readonly Assembly AppAssembly = typeof(ServiceRegistrations).Assembly;

    [Test]
    public void AppShellOwnsPortalShellComponents()
    {
        AssertTypesExist(
            "BlazorServer.AppShell.App",
            "BlazorServer.AppShell.AppRoutes",
            "BlazorServer.AppShell.Layout.MainLayout",
            "BlazorServer.AppShell.Layout.ReconnectModal",
            "BlazorServer.AppShell.Layout.PageSetupWithNavigation",
            "BlazorServer.AppShell.Navigation.NavMenu",
            "BlazorServer.AppShell.Pages.Home",
            "BlazorServer.AppShell.Pages.NotFound",
            "BlazorServer.AppShell.Pages.Error",
            "BlazorServer.AppShell.StateManagement.GlobalStateManager");
    }

    [Test]
    public void AppShellAuthenticationSeparatesSharedEntraAndMockTypes()
    {
        AssertTypesExist(
            "BlazorServer.AppShell.Authentication.AuthenticationRegistration",
            "BlazorServer.AppShell.Authentication.AuthenticationActions",
            "BlazorServer.AppShell.Authentication.BlazorCurrentUserContext",
            "BlazorServer.AppShell.Authentication.Entra.EntraAuthenticationRegistration",
            "BlazorServer.AppShell.Authentication.Entra.GraphClientAuthProvider",
            "BlazorServer.AppShell.Authentication.Mock.MockAuthenticationRegistration",
            "BlazorServer.AppShell.Authentication.Mock.MockAuthHandler",
            "BlazorServer.AppShell.Authentication.Mock.MockAuthenticationState",
            "BlazorServer.AppShell.Authentication.Mock.MockCurrentUserContext",
            "BlazorServer.AppShell.Authentication.Mock.MockCurrentUserSwitcher",
            "BlazorServer.AppShell.Authentication.Mock.MockedLoginDisplay");

        AssertTypesDoNotExist("BlazorServer.AppShell.Authentication.GraphClientAuthProvider");
    }

    [Test]
    public void UiKitOwnsNeutralUiPrimitives()
    {
        AssertTypesExist(
            "BlazorServer.UiKit.StyleComponent",
            "BlazorServer.UiKit.ToolbarPage",
            "BlazorServer.UiKit.IsActiveIconButton",
            "BlazorServer.UiKit.NavButton",
            "BlazorServer.UiKit.Input.KipTextField`1",
            "BlazorServer.UiKit.Input.KipSelect`1",
            "BlazorServer.UiKit.Input.KipDropdown");
    }

    [Test]
    public void MovedShellAndUiPrimitivesAreNoLongerSharedTypes()
    {
        AssertTypesDoNotExist(
            "BlazorServer.Shared.Components.App",
            "BlazorServer.Shared.Components.Routes",
            "BlazorServer.Shared.Components.Layout.MainLayout",
            "BlazorServer.Shared.Components.PageSetupWithNavigation",
            "BlazorServer.Shared.Components.Navigation.NavMenu",
            "BlazorServer.Shared.Components.Navigation.NavButton",
            "BlazorServer.Shared.Components.Pages.Home",
            "BlazorServer.Shared.Components.StateManagment.GlobalStateManager",
            "BlazorServer.Shared.Components.Input.KipTextField`1",
            "BlazorServer.Shared.Components.Dialogs.StringInputDialog",
            "BlazorServer.Shared.Components.ToolbarPage",
            "BlazorServer.Shared.Components.IsActiveIconButton",
            "BlazorServer.Shared.Abstractions.StyleComponent");
    }

    [Test]
    public void UiKitPublicSurfaceDoesNotDependOnAppShellOrModules()
    {
        var forbiddenPrefixes = new[] { "BlazorServer.AppShell", "BlazorServer.Modules" };
        var offendingTypes = AppAssembly
            .GetTypes()
            .Where(type => type.Namespace?.StartsWith("BlazorServer.UiKit", StringComparison.Ordinal) == true)
            .SelectMany(type => PublicSurfaceTypes(type).Select(reference => $"{type.FullName} -> {reference.FullName}"))
            .Where(reference => forbiddenPrefixes.Any(reference.Contains))
            .ToArray();

        Assert.That(offendingTypes, Is.Empty);
    }

    private static void AssertTypesExist(params string[] typeNames)
    {
        var missingTypes = typeNames
            .Where(typeName => AppAssembly.GetType(typeName) is null)
            .ToArray();

        Assert.That(missingTypes, Is.Empty);
    }

    private static void AssertTypesDoNotExist(params string[] typeNames)
    {
        var existingTypes = typeNames
            .Where(typeName => AppAssembly.GetType(typeName) is not null)
            .ToArray();

        Assert.That(existingTypes, Is.Empty);
    }

    private static IEnumerable<Type> PublicSurfaceTypes(Type type)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        foreach (var property in type.GetProperties(flags))
        {
            yield return property.PropertyType;
        }

        foreach (var field in type.GetFields(flags))
        {
            yield return field.FieldType;
        }

        foreach (var method in type.GetMethods(flags))
        {
            yield return method.ReturnType;

            foreach (var parameter in method.GetParameters())
            {
                yield return parameter.ParameterType;
            }
        }
    }
}
