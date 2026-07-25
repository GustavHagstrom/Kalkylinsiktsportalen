using System.Reflection;
using BlazorServer;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Views.Rendering;
using BlazorServer.Modules.Reports.Views.Editing;
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
            Assert.That(typeof(IViewStore).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb"));
            Assert.That(typeof(MongoView).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb"));
        });
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
