using System.Reflection;
using BlazorServer;
using BlazorServer.Modules.Reports.Components.PrintPreview;
using BlazorServer.Modules.Reports.Components.Views;
using BlazorServer.Modules.Reports.Infrastructure.Persistence;
using BlazorServer.Modules.Reports.Infrastructure.Persistence.Entities;
using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection;
using BlazorServer.Modules.Reports.Public.Contracts.Views.SheetSection;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ReportsViewOwnershipTests
{
    private static readonly Assembly AppAssembly = typeof(ServiceAndFeatureRegistrations).Assembly;

    [Test]
    public void ReportsOwnsViewContractsComponentsAndPersistence()
    {
        Assert.Multiple(() =>
        {
            Assert.That(typeof(ViewDto).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Public.Contracts.Views"));
            Assert.That(typeof(DataSectionDto).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection"));
            Assert.That(typeof(SheetSectionDto).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Public.Contracts.Views.SheetSection"));
            Assert.That(typeof(ViewEditor).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Components.Views"));
            Assert.That(typeof(PrintableDocument).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Components.PrintPreview"));
            Assert.That(typeof(IViewStore).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Infrastructure.Persistence"));
            Assert.That(typeof(MongoView).Namespace, Is.EqualTo("BlazorServer.Modules.Reports.Infrastructure.Persistence.Entities"));
        });
    }

    [Test]
    public void LegacySharedViewTypesAreNoLongerProductionTypes()
    {
        var oldTypeNames = new[]
        {
            "BlazorServer.Shared.DTOs.Views.ViewDto",
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
