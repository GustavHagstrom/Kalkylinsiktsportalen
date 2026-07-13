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
                && !remainder.StartsWith("Public", StringComparison.Ordinal),
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
                    && !remainder.StartsWith("Public", StringComparison.Ordinal))
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
                    && !remainder.StartsWith("Public", StringComparison.Ordinal))
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
    public void CentralMongoInfrastructureMustNotKnowModulePersistence()
        => AssertNoReferences(
            FilesUnder(Path.Combine("Infrastructure", "MongoDb")),
            reference => reference.StartsWith("BlazorServer.Modules", StringComparison.Ordinal),
            "Central Infrastructure/MongoDb must not depend on module-owned stores, entities or collection definitions");

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

    [GeneratedRegex(@"\bBlazorServer(?:\.[A-Za-z_][A-Za-z0-9_]*)+")]
    private static partial Regex NamespaceReferenceRegex();

    [GeneratedRegex(@"^BlazorServer\.Modules\.(?<module>[A-Za-z_][A-Za-z0-9_]*)(?:\.(?<remainder>.+))?$")]
    private static partial Regex ModuleReferenceRegex();
}
