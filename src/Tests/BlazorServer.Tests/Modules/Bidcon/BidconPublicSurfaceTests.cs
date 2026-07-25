using System.Reflection;
using BlazorServer;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class BidconPublicSurfaceTests
{
    private static readonly Assembly AppAssembly = typeof(ServiceRegistrations).Assembly;

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

    [Test]
    public void UpsertBidconCredentialsSliceDoesNotOwnHttpCredentialVerificationAdapter()
    {
        var forbiddenReferences = new[]
        {
            "IHttpClientFactory",
            "BidconHttpClientNames",
            "BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs",
            "ApiKeyAuthenticateDto",
            "ApiKeyAuthenticateResultDto"
        };
        var violations = SourceFilesUnder(
                "Modules",
                "Bidcon",
                "Features",
                "UpsertBidconCredentials")
            .SelectMany(file =>
            {
                var text = File.ReadAllText(file);
                return forbiddenReferences
                    .Where(text.Contains)
                    .Select(reference => $"{Path.GetFileName(file)} -> {reference}");
            })
            .ToArray();

        Assert.That(violations, Is.Empty,
            "The UpsertBidconCredentials slice should keep only the verifier port; HTTP and upstream Bidcon auth DTOs belong in infrastructure.\n"
            + string.Join('\n', violations));
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

    private static IEnumerable<string> SourceFilesUnder(params string[] pathParts)
    {
        var directory = Path.Combine(new[] { FindProductionRoot() }.Concat(pathParts).ToArray());
        return Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories);
    }

    private static string FindProductionRoot()
    {
        for (var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "src", "BlazorServer");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("Could not locate src/BlazorServer from the test output directory.");
    }
}
