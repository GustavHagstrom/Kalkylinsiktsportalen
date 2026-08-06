using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ViewTests
{
    [Test]
    public void AddDataSection_AddsDefaultEditableGridAtNextSectionIndex()
    {
        var view = new View
        {
            SheetSections = new List<SheetSection>
            {
                new() { SectionIndex = 0 }
            }
        };
        var section = view.AddDataSection();

        Assert.Multiple(() =>
        {
            Assert.That(section.SectionIndex, Is.EqualTo(1));
            Assert.That(section.RowsCount, Is.EqualTo(3));
            Assert.That(section.ColumnsCount, Is.EqualTo(3));
            Assert.That(section.DataCells.Keys, Is.EquivalentTo(new[]
            {
                "R0C0", "R0C1", "R0C2",
                "R1C0", "R1C1", "R1C2",
                "R2C0", "R2C1", "R2C2"
            }));
        });
    }

    [Test]
    public void RemoveDataSection_CollapsesFollowingSectionIndexes()
    {
        var removedSection = new DataSection { SectionIndex = 0 };
        var remainingSection = new SheetSection { SectionIndex = 1 };
        var view = new View
        {
            DataSections = new List<DataSection> { removedSection },
            SheetSections = new List<SheetSection> { remainingSection }
        };
        view.RemoveDataSection(removedSection);

        Assert.Multiple(() =>
        {
            Assert.That(view.DataSections, Is.Empty);
            Assert.That(remainingSection.SectionIndex, Is.EqualTo(0));
        });
    }
}
