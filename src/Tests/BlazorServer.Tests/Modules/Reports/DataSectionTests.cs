using BlazorServer.Modules.Reports.Domain.Views.DataSections;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class DataSectionTests
{
    [Test]
    public void AddRowAndColumn_DefinesExplicitGridWithoutMaterializingDefaultCells()
    {
        var section = DataSection.Create(1, 1);

        section.AddColumn();
        section.AddRow();

        Assert.Multiple(() =>
        {
            Assert.That(section.Rows, Has.Count.EqualTo(2));
            Assert.That(section.Columns, Has.Count.EqualTo(2));
            Assert.That(section.Cells, Is.Empty);
        });
    }

    [Test]
    public void RemoveRowAndColumn_CollapsesIndexesAndCells()
    {
        var section = DataSection.CreateDefault();

        section.RemoveRow(1);
        section.RemoveColumn(1);

        Assert.Multiple(() =>
        {
            Assert.That(section.Rows, Has.Count.EqualTo(2));
            Assert.That(section.Columns, Has.Count.EqualTo(2));
            Assert.That(section.Cells, Is.Empty);
        });
    }

    [Test]
    public void EditCell_StoresOnlyCellsThatDifferFromDefault()
    {
        var section = DataSection.Create(2, 2);

        section.EditCellExpression(1, 1, "Total");
        section.EditCellExpression(1, 1, "");

        Assert.Multiple(() =>
        {
            Assert.That(section.GetCell(0, 0).CellExpression, Is.Empty);
            Assert.That(section.Cells, Is.Empty);
        });
    }

    [Test]
    public void EditCell_RejectsPositionsOutsideGridBounds()
    {
        var section = DataSection.Create(1, 1);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            section.EditCellExpression(1, 0, "Outside"));

        Assert.That(exception!.ParamName, Is.EqualTo("rowIndex"));
    }

    [Test]
    public void RemoveRowAndColumn_DoNotRemoveTheLastGridLine()
    {
        var section = DataSection.Create(1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(() => section.RemoveRow(0), Throws.TypeOf<InvalidOperationException>());
            Assert.That(() => section.RemoveColumn(0), Throws.TypeOf<InvalidOperationException>());
        });
    }

    [Test]
    public void RowAndColumnLayout_UseMillimetersAndRelativeWeights()
    {
        var section = DataSection.Create(1, 1);
        var row = section.AddRow(12.5);
        var column = section.AddColumn(2.5);

        Assert.Multiple(() =>
        {
            Assert.That(row.HeightMm, Is.EqualTo(12.5));
            Assert.That(column.RelativeWidth, Is.EqualTo(2.5));
        });
    }
}
