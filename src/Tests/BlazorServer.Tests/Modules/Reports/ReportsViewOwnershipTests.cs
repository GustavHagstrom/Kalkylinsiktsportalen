using System.Reflection;
using BlazorServer;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Views.Rendering;
using BlazorServer.Modules.Reports.Views.Editing;
using BlazorServer.Modules.Reports.Views;
using BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb;
using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection;
using BlazorServer.Modules.Reports.Public.Contracts.Views.SheetSection;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ReportsViewOwnershipTests
{
    private static readonly Assembly AppAssembly = typeof(ServiceRegistrations).Assembly;

    [Test]
    public void ReportsOwnsViewContractsComponentsAndPersistence()
    {
        Assert.Multiple(() =>
        {
            Assert.That(typeof(ViewDto).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Public.Contracts.Views"));
            Assert.That(typeof(ViewId).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Public"));
            Assert.That(typeof(DataSectionDto).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection"));
            Assert.That(typeof(SheetSectionDto).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Public.Contracts.Views.SheetSection"));
            Assert.That(typeof(ViewEditor).Namespace, Does.StartWith("BlazorServer.Modules.Reports."));
            Assert.That(typeof(ViewEditor).Namespace, Does.Not.Contain(".Components"));
            Assert.That(typeof(DataCellFormulaEvaluator).Namespace, Does.StartWith("BlazorServer.Modules.Reports."));
            Assert.That(typeof(DataCellFormulaEvaluator).Namespace, Does.Not.Contain(".Components"));
            Assert.That(typeof(FormatBuilder).Namespace, Does.StartWith("BlazorServer.Modules.Reports."));
            Assert.That(typeof(FormatBuilder).Namespace, Does.Not.Contain(".Components"));
            Assert.That(typeof(PrintableDocument).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Views.Rendering"));
            Assert.That(typeof(IViewStore).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Views"));
            Assert.That(typeof(MongoView).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb"));
            Assert.That(typeof(MongoViewStore).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb"));
            Assert.That(typeof(ViewMappings).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb"));
        });
    }

    [Test]
    public void ReportsComponentsDoNotDependOnViewPersistencePortDirectly()
    {
        var componentFiles = Directory
            .EnumerateFiles(
                Path.Combine(FindProductionRoot(), "Modules", "Reports", "Components"),
                "*.razor",
                SearchOption.AllDirectories)
            .Select(Path.GetFullPath);

        var violations = componentFiles
            .Where(file => File.ReadAllText(file).Contains("IViewStore", StringComparison.Ordinal))
            .Select(Path.GetFileName)
            .ToArray();

        Assert.That(violations, Is.Empty);
    }

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

    [Test]
    public void LegacySharedViewTypesAreNoLongerProductionTypes()
    {
        var oldTypeNames = new[]
        {
            "BlazorServer.Shared.DTOs.Views.ViewDto",
            "BlazorServer.Kernel.ViewId",
            "BlazorServer.Shared.ViewId",
            "BlazorServer.Shared.DTOs.Views.DataSection.DataSectionDto",
            "BlazorServer.Shared.DTOs.Views.SheetSection.SheetSectionDto",
            "BlazorServer.Shared.Components.Views.ViewEditor",
            "BlazorServer.Shared.Components.Views.DataCellFormulaEvaluator",
            "BlazorServer.Shared.Components.PrintPreview.PrintableDocument",
            "BlazorServer.Shared.Stores.IViewStore",
            "BlazorServer.Shared.Stores.MongoDb.Enteties.MongoView",
            "BlazorServer.Shared.Stores.MongoDb.Mappers.MongoViewMapper",
            "BlazorServer.Shared.Stores.MongoDb.Stores.MongoViewStore"
        };

        var existingTypes = oldTypeNames
            .Where(typeName => AppAssembly.GetType(typeName) is not null)
            .ToArray();

        Assert.That(existingTypes, Is.Empty);
    }
}
