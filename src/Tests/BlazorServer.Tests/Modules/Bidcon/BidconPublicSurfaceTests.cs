using System.Reflection;
using BlazorServer;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class BidconPublicSurfaceTests
{
    private static readonly Assembly AppAssembly = typeof(ServiceAndFeatureRegistrations).Assembly;

    [Test]
    public void BidconKeepsUpstreamDtosPrivateToInfrastructure()
    {
        AssertTypesExist(
            "BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs.EstimationDto",
            "BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs.EstimationDetailsDto",
            "BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients.BidconEstimationClient",
            "BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Authentication.BidconHttpClientNames");

        AssertTypesDoNotExist(
            "BlazorServer.Shared.DTOs.Bidcon.EstimationDto",
            "BlazorServer.Shared.DTOs.Bidcon.ResourceDto",
            "BlazorServer.Shared.Models.Bidcon.BidconApiCredentials",
            "BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs.EstimationDto");

        var publicIntegrationTypes = AppAssembly
            .GetTypes()
            .Where(type => type.Namespace?.StartsWith("BlazorServer.Modules.Bidcon.Infrastructure.BidconApi", StringComparison.Ordinal) == true)
            .Where(type => type.IsPublic || type.IsNestedPublic)
            .Select(type => type.FullName)
            .ToArray();

        Assert.That(publicIntegrationTypes, Is.Empty);
    }

    [Test]
    public void NonBidconModulesDoNotReferenceBidconInfrastructure()
    {
        var offendingTypes = AppAssembly
            .GetTypes()
            .Where(type => type.Namespace is not null)
            .Where(type => !type.Namespace!.StartsWith("BlazorServer.Modules.Bidcon", StringComparison.Ordinal))
            .SelectMany(type => PublicSurfaceTypes(type).Select(reference => $"{type.FullName} -> {reference.FullName}"))
            .Where(reference => reference.Contains("BlazorServer.Modules.Bidcon.Infrastructure", StringComparison.Ordinal))
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
