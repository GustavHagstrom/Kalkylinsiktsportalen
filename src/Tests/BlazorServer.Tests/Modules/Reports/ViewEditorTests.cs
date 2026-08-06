using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using BlazorServer.Modules.Reports.Domain.Views.Editing;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ViewEditorTests
{
    [Test]
    public void AddDataSection_AddsDefaultEditableGridAtNextSectionIndex()
    {
        var view = new ViewDto
        {
            SheetSections = new List<SheetSectionDto>
            {
                new() { SectionIndex = 0 }
            }
        };
        var editor = new ViewEditor(view);

        var section = editor.AddDataSection();

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
        var removedSection = new DataSectionDto { SectionIndex = 0 };
        var remainingSection = new SheetSectionDto { SectionIndex = 1 };
        var view = new ViewDto
        {
            DataSections = new List<DataSectionDto> { removedSection },
            SheetSections = new List<SheetSectionDto> { remainingSection }
        };
        var editor = new ViewEditor(view);

        editor.RemoveDataSection(removedSection);

        Assert.Multiple(() =>
        {
            Assert.That(view.DataSections, Is.Empty);
            Assert.That(remainingSection.SectionIndex, Is.EqualTo(0));
        });
    }
}
