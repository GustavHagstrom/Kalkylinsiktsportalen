using NetArchTest.Rules;
using System.Text.RegularExpressions;

namespace BlazorServer.Tests.Architecture;

[TestFixture]
public sealed partial class FinalArchitectureBoundaryTests
{
    private static readonly string ProductionRoot = FindProductionRoot();

    [Test]
    public void SharedMustNotExistInProductionCode()
    {
        var violations = SourceFiles()
            .Where(file => RelativePath(file).Split('/').Contains("Shared", StringComparer.OrdinalIgnoreCase)
                || SharedNamespaceRegex().IsMatch(File.ReadAllText(file)))
            .Select(RelativePath)
            .ToArray();

        Assert.That(violations, Is.Empty,
            "Shared is retired. Move each production type to an explicit owner. Violations:\n" + string.Join('\n', violations));
    }

    [Test]
    public void KernelMustRemainIndependentAndTechnologyNeutral()
    {
        AssertNoReferences(
            files: FilesUnder("Kernel"),
            forbidden: reference => StartsWithAny(reference,
                "BlazorServer.AppShell", "BlazorServer.Infrastructure", "BlazorServer.UiKit", "BlazorServer.Modules"),
            rule: "Kernel must not depend on AppShell, Infrastructure, UiKit or Modules");

        var allowedAreas = new[] { "CQRS", "Identity", "Validation" };
        var unexpectedAreas = FilesUnder("Kernel")
            .Select(file => RelativePath(file).Split('/')[1])
            .Where(area => !allowedAreas.Contains(area, StringComparer.Ordinal))
            .Distinct()
            .ToArray();

        Assert.That(unexpectedAreas, Is.Empty,
            "Kernel may only contain approved technology-neutral areas. Unexpected areas: " + string.Join(", ", unexpectedAreas));
    }

    [Test]
    public void UiKitMustNotKnowAboutModulesOrDomainTypes()
        => AssertNoReferences(
            FilesUnder("UiKit"),
            reference => reference.StartsWith("BlazorServer.Modules", StringComparison.Ordinal),
            "UiKit must not depend on modules or domain-specific module types");

    [Test]
    public void AppShellMayOnlyConsumeModulePublicContracts()
        => AssertNoReferences(
            FilesUnder("AppShell"),
            reference => IsModuleReference(reference, out _, out var remainder)
                && !IsPublicModuleRemainder(remainder),
            "AppShell may compose modules only through Modules.<Module>.Public contracts");

    [Test]
    public void ModulesMayOnlyConsumeOtherModulesThroughPublicContracts()
    {
        var violations = new List<string>();
        foreach (var file in FilesUnder("Modules"))
        {
            var owner = RelativePath(file).Split('/')[1];
            foreach (var reference in NamespaceReferences(file))
            {
                if (IsModuleReference(reference, out var target, out var remainder)
                    && !string.Equals(owner, target, StringComparison.Ordinal)
                    && !IsPublicModuleRemainder(remainder))
                {
                    violations.Add($"{RelativePath(file)} -> {reference}");
                }
            }
        }

        Assert.That(violations, Is.Empty,
            "A module may consume another module only through its Public namespace. Violations:\n" + string.Join('\n', violations));
    }

    [Test]
    public void ModulePublicContractsMustNotExposeOwnerInternals()
    {
        var violations = new List<string>();
        var publicContractFiles = FilesUnder("Modules")
            .Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .Where(file => RelativePath(file).Split('/').ElementAtOrDefault(2) == "Public");

        foreach (var file in publicContractFiles)
        {
            var owner = RelativePath(file).Split('/')[1];
            foreach (var reference in NamespaceReferences(file))
            {
                if (IsModuleReference(reference, out var target, out var remainder)
                    && string.Equals(owner, target, StringComparison.Ordinal)
                    && !IsPublicModuleRemainder(remainder))
                {
                    violations.Add($"{RelativePath(file)} exposes {reference}");
                }
            }
        }

        Assert.That(violations, Is.Empty,
            "Module Public contracts must not expose internal DTOs, stores, persistence entities or implementations. Violations:\n"
            + string.Join('\n', violations));
    }

    [Test]
    public void ModulePublicComponentsMustNotReferenceOwnerInternals()
    {
        var violations = new List<string>();
        var publicComponentFiles = FilesUnder("Modules")
            .Where(file => RelativePath(file).Split('/').ElementAtOrDefault(2) == "Public")
            .Where(file => RelativePath(file).Split('/').ElementAtOrDefault(3) == "Components");

        foreach (var file in publicComponentFiles)
        {
            var owner = RelativePath(file).Split('/')[1];
            foreach (var reference in NamespaceReferences(file))
            {
                if (IsModuleReference(reference, out var target, out var remainder)
                    && string.Equals(owner, target, StringComparison.Ordinal)
                    && !IsPublicModuleRemainder(remainder))
                {
                    violations.Add($"{RelativePath(file)} references {reference}");
                }
            }

            foreach (var reference in RelativeOwnerInternalReferences(file))
            {
                violations.Add($"{RelativePath(file)} references {reference}");
            }
        }

        Assert.That(violations, Is.Empty,
            "Modules/{Module}/Public/Components are stable Blazor entrypoints. They may expose only stable parameters, callbacks and public contracts; orchestration and owner internals belong in the module's internal Components or feature handlers. Violations:\n"
            + string.Join('\n', violations));
    }

    [Test]
    public void ModulesMayNotReferenceAppShell() 
    {
        var assembly = typeof(Program).Assembly;
        var result = Types.InAssembly(assembly).That().ResideInNamespaceMatching(@"^BlazorServer\.Modules(\.|$)").ShouldNot().HaveDependencyOn("BlazorServer.AppShell").GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Modules must not depend on AppShell. Violations:\n" + string.Join('\n', result.FailingTypeNames ?? Array.Empty<string>()));
    }
    [Test]
    public void CompositionShouldNotReferenceOtherLayers()
    {
        var assembly = typeof(Program).Assembly;
        var result = Types.InAssembly(assembly).That().ResideInNamespaceMatching(@"^BlazorServer\.Composition(\.|$)").ShouldNot()
            .HaveDependencyOnAny(
            "BlazorServer.Modules.Access.Domain",
            "BlazorServer.Modules.Access.Features",
            "BlazorServer.Modules.Access.Infrastructure",
            "BlazorServer.Modules.Access.Components",

            "BlazorServer.Modules.Bidcon.Domain",
            "BlazorServer.Modules.Bidcon.Features",
            "BlazorServer.Modules.Bidcon.Infrastructure",
            "BlazorServer.Modules.Bidcon.Components",

            "BlazorServer.Modules.Projects.Domain",
            "BlazorServer.Modules.Projects.Features",
            "BlazorServer.Modules.Projects.Infrastructure",
            "BlazorServer.Modules.Projects.Components",

            "BlazorServer.Modules.Reports.Domain",
            "BlazorServer.Modules.Reports.Features",
            "BlazorServer.Modules.Reports.Infrastructure",
            "BlazorServer.Modules.Reports.Components",

            "BlazorServer.AppShell",
            "BlazorServer.Kernel",
            "BlazorServer.UiKit",
            "BlazorServer.Infrastructure",
            "BlazorServer.Identity")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Composition must be self-contained. Violations:\n" + string.Join('\n', result.FailingTypeNames ?? Array.Empty<string>()));
    }
    [Test]
    public void KernelShouldNotReferenceOtherLayers()
    {
        var assembly = typeof(Program).Assembly;
        var result = Types.InAssembly(assembly).That().ResideInNamespaceMatching(@"^BlazorServer\.Kernel(\.|$)").ShouldNot()
            .HaveDependencyOnAll(
            "BlazorServer.Modules",
            "BlazorServer.AppShell",
            "BlazorServer.Composition",
            "BlazorServer.UiKit",
            "BlazorServer.Infrastructure",
            "BlazorServer.Identity").GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Kernel must be self-contained. Violations:\n" + string.Join('\n', result.FailingTypeNames ?? Array.Empty<string>()));
    }
    [Test]
    public void CentralMongoInfrastructureMustNotKnowModulePersistence()
        => AssertNoReferences(
            FilesUnder(Path.Combine("Infrastructure", "MongoDb")),
            reference => reference.StartsWith("BlazorServer.Modules", StringComparison.Ordinal),
            "Central Infrastructure/MongoDb must not depend on module-owned stores, entities or collection definitions");

    [Test]
    public void FeatureSlicesMustNotContainRazorComponents()
    {
        var violations = FeatureSliceFiles()
            .Where(file => file.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            .Select(RelativePath)
            .ToArray();

        Assert.That(violations, Is.Empty,
            "Feature slices are UI-independent use cases. Move .razor components to the module's Components area, or to Modules/{Module}/Public/Components when the component is a stable public entrypoint. Violations:\n"
            + string.Join('\n', violations));
    }

    [Test]
    public void FeatureSlicesMustNotReferenceBlazorComponentNamespaces()
        => AssertNoReferences(
            FeatureSliceFiles(),
            reference => reference.StartsWith("Microsoft.AspNetCore.Components", StringComparison.Ordinal)
                || IsModuleReference(reference, out _, out var remainder)
                    && IsComponentNamespaceRemainder(remainder),
            "Feature slices are UI-independent. Blazor component namespaces and UI services belong in module Components or Public/Components, while slices expose commands, queries, handlers and result contracts");

    [Test]
    public void FeatureSlicesMustNotReferenceAdapterTechnologiesDirectly()
        => AssertNoReferences(
            FeatureSliceFiles(),
            IsForbiddenFeatureAdapterTechnology,
            "Feature slices must depend on slice-owned ports or module contracts, not adapter technologies. MongoDB, Microsoft Graph, IHttpClientFactory and other external adapter APIs belong in module Infrastructure");

    private static void AssertNoReferences(IEnumerable<string> files, Func<string, bool> forbidden, string rule)
    {
        var violations = files
            .SelectMany(file => NamespaceReferences(file)
                .Where(forbidden)
                .Select(reference => $"{RelativePath(file)} -> {reference}"))
            .Distinct()
            .ToArray();

        Assert.That(violations, Is.Empty, rule + ". Violations:\n" + string.Join('\n', violations));
    }

    private static IEnumerable<string> NamespaceReferences(string file)
    {
        var text = File.ReadAllText(file);
        return NamespaceReferenceRegex().Matches(text)
            .Select(match => match.Value.TrimEnd('.'))
            .Distinct(StringComparer.Ordinal);
    }

    private static bool IsModuleReference(string reference, out string module, out string remainder)
    {
        var match = ModuleReferenceRegex().Match(reference);
        module = match.Success ? match.Groups["module"].Value : string.Empty;
        remainder = match.Success ? match.Groups["remainder"].Value : string.Empty;
        return match.Success;
    }

    private static bool StartsWithAny(string value, params string[] prefixes)
        => prefixes.Any(prefix => value.StartsWith(prefix, StringComparison.Ordinal));

    private static bool IsPublicModuleRemainder(string remainder)
        => remainder.Equals("Public", StringComparison.Ordinal)
            || remainder.StartsWith("Public.", StringComparison.Ordinal);

    private static bool IsComponentNamespaceRemainder(string remainder)
        => remainder.Equals("Components", StringComparison.Ordinal)
            || remainder.StartsWith("Components.", StringComparison.Ordinal)
            || remainder.Contains(".Components.", StringComparison.Ordinal);

    private static bool IsForbiddenFeatureAdapterTechnology(string reference)
        => reference.StartsWith("MongoDB.", StringComparison.Ordinal)
            || reference.StartsWith("Microsoft.Graph", StringComparison.Ordinal)
            || reference.Equals("System.Net.Http.IHttpClientFactory", StringComparison.Ordinal)
            || reference.StartsWith("Microsoft.AspNetCore.Components", StringComparison.Ordinal);

    private static IEnumerable<string> RelativeOwnerInternalReferences(string file)
    {
        var text = File.ReadAllText(file);
        return RelativeOwnerInternalReferenceRegex().Matches(text)
            .Select(match => match.Value.TrimEnd('.'))
            .Distinct(StringComparer.Ordinal);
    }

    private static IEnumerable<string> SourceFiles()
        => Directory.EnumerateFiles(ProductionRoot, "*.*", SearchOption.AllDirectories)
            .Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                || file.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                && !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"));

    private static IEnumerable<string> FilesUnder(string relativeDirectory)
    {
        var directory = Path.Combine(ProductionRoot, relativeDirectory);
        return Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                    || file.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            : [];
    }

    private static IEnumerable<string> FeatureSliceFiles()
        => FilesUnder("Modules")
            .Where(file => RelativePath(file).Split('/').Contains("Features", StringComparer.Ordinal));

    private static string RelativePath(string file)
        => Path.GetRelativePath(ProductionRoot, file).Replace('\\', '/');

    private static string FindProductionRoot()
    {
        for (var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "src", "BlazorServer");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("Could not locate src/BlazorServer from the test output directory.");
    }

    [GeneratedRegex(@"\bnamespace\s+BlazorServer\.Shared(?:\.|\b)|\bBlazorServer\.Shared(?:\.|\b)")]
    private static partial Regex SharedNamespaceRegex();

    [GeneratedRegex(@"\b(?:BlazorServer|Microsoft|MongoDB|System)(?:\.[A-Za-z_][A-Za-z0-9_]*)+|\bIHttpClientFactory\b")]
    private static partial Regex NamespaceReferenceRegex();

    [GeneratedRegex(@"^BlazorServer\.Modules\.(?<module>[A-Za-z_][A-Za-z0-9_]*)(?:\.(?<remainder>.+))?$")]
    private static partial Regex ModuleReferenceRegex();

    [GeneratedRegex(@"(?<![A-Za-z0-9_.])(?:Features|Domain|Infrastructure|Components)(?:\.[A-Za-z_][A-Za-z0-9_]*)+")]
    private static partial Regex RelativeOwnerInternalReferenceRegex();
}
