using System.Text.RegularExpressions;

namespace BlazorServer.Tests.Architecture;

[TestFixture]
public sealed partial class RootCompositionBoundaryTests
{
    private static readonly string ProductionRoot = FindProductionRoot();

    [Test]
    public void GlobalRazorImportsMayOnlyExposeModulePublicContracts()
    {
        var importsFile = Path.Combine(ProductionRoot, "_Imports.razor");

        AssertNoForbiddenModuleReferences(
            importsFile,
            allowModuleRoot: false,
            "Global Razor imports may only reference Modules.<Module>.Public contracts");
    }

    [Test]
    public void BootstrapMayOnlyReferenceModuleEntrypointsOrPublicContracts()
    {
        var bootstrapFile = Path.Combine(ProductionRoot, "ServiceRegistrations.cs");

        AssertNoForbiddenModuleReferences(
            bootstrapFile,
            allowModuleRoot: true,
            "Bootstrap may reference module registration entrypoints or Modules.<Module>.Public contracts");
    }

    private static void AssertNoForbiddenModuleReferences(
        string file,
        bool allowModuleRoot,
        string rule)
    {
        var violations = ModuleReferenceRegex().Matches(File.ReadAllText(file))
            .Select(match => new
            {
                Reference = match.Value,
                Remainder = match.Groups["remainder"].Value
            })
            .Where(match => !(allowModuleRoot && string.IsNullOrEmpty(match.Remainder)))
            .Where(match => !match.Remainder.StartsWith("Public", StringComparison.Ordinal))
            .Select(match => match.Reference)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.That(
            violations,
            Is.Empty,
            rule + ". Violations:\n" + string.Join('\n', violations));
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

        throw new DirectoryNotFoundException(
            "Could not locate src/BlazorServer from the test output directory.");
    }

    [GeneratedRegex(@"\bBlazorServer\.Modules\.(?<module>[A-Za-z_][A-Za-z0-9_]*)(?:\.(?<remainder>[A-Za-z_][A-Za-z0-9_.]*))?")]
    private static partial Regex ModuleReferenceRegex();
}
