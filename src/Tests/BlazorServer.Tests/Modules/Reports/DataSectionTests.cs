using BlazorServer.Modules.Reports.Domain.Views.DataSection;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class DataSectionTests
{
    [Test]
    public void AddRowAndColumn_MaintainsCompleteCellGrid()
    {
        var section = new DataSection();

        section.AddColumn();
        section.AddColumn();
        section.AddRow();
        section.AddRow();

        Assert.Multiple(() =>
        {
            Assert.That(section.RowsCount, Is.EqualTo(2));
            Assert.That(section.ColumnsCount, Is.EqualTo(2));
            Assert.That(section.DataCells.Keys, Is.EquivalentTo(new[] { "R0C0", "R0C1", "R1C0", "R1C1" }));
        });
    }

    [Test]
    public void RemoveRowAndColumn_CollapsesIndexesAndCells()
    {
        var section = DataSection.CreateDefault(sectionIndex: 0);

        section.RemoveRow(1);
        section.RemoveColumn(1);

        Assert.Multiple(() =>
        {
            Assert.That(section.RowDefinitions.Select(row => row.RowIndex), Is.EqualTo(new[] { 0, 1 }));
            Assert.That(section.ColumnDefinitions.Select(column => column.ColumnIndex), Is.EqualTo(new[] { 0, 1 }));
            Assert.That(section.DataCells.Keys, Is.EquivalentTo(new[] { "R0C0", "R0C1", "R1C0", "R1C1" }));
        });
    }
}
