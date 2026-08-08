using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSection;

public class DataSection
{
    private const double DefaultRowHeight = 8;
    private const int DefaultColumnWidth = 10;

    public string SectionId { get; set; } = SectionIds.New();
    public int SectionIndex { get; set; } = 0;
    public int RowsCount { get; set; } = 0;
    public int ColumnsCount { get; set; } = 0;
    /// <summary>
    /// Key: "R{rowIndex}C{columnIndex}" 0 based arrays
    /// </summary>
    public Dictionary<string, DataCell> DataCells { get; set; } = new();
    public List<RowDefinition> RowDefinitions { get; set; } = new();
    public List<DataColumnDefinition> ColumnDefinitions { get; set; } = new();

    public IReadOnlyList<RowDefinition> Rows => RowDefinitions;
    public IReadOnlyList<DataColumnDefinition> Columns => ColumnDefinitions;

    public static DataSection CreateDefault(int sectionIndex)
    {
        var section = new DataSection
        {
            SectionIndex = sectionIndex
        };

        section.AddColumn();
        section.AddColumn();
        section.AddColumn();
        section.AddRow();
        section.AddRow();
        section.AddRow();

        return section;
    }

    public void Initialize(int rowsCount, int columnsCount)
    {
        if (rowsCount < 0)
            throw new ArgumentOutOfRangeException(nameof(rowsCount));

        if (columnsCount < 0)
            throw new ArgumentOutOfRangeException(nameof(columnsCount));

        RowDefinitions.Clear();
        ColumnDefinitions.Clear();
        DataCells.Clear();
        RowsCount = 0;
        ColumnsCount = 0;

        for (var i = 0; i < rowsCount; i++)
            AddRow();

        for (var i = 0; i < columnsCount; i++)
            AddColumn();
    }

    public RowDefinition AddRow(double height = DefaultRowHeight)
    {
        Normalize();

        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        var row = new RowDefinition
        {
            RowIndex = RowsCount,
            Height = height
        };

        RowDefinitions.Add(row);
        RowsCount = RowDefinitions.Count;

        for (var columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
            EnsureCellExists(row.RowIndex, columnIndex);

        return row;
    }

    public DataColumnDefinition AddColumn(int width = DefaultColumnWidth)
    {
        Normalize();

        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        var column = new DataColumnDefinition
        {
            ColumnIndex = ColumnsCount,
            Width = width
        };

        ColumnDefinitions.Add(column);
        ColumnsCount = ColumnDefinitions.Count;

        for (var rowIndex = 0; rowIndex < RowsCount; rowIndex++)
            EnsureCellExists(rowIndex, column.ColumnIndex);

        return column;
    }

    public void RemoveRow(int rowIndex)
    {
        Normalize();
        ValidateRowIndex(rowIndex);

        var rowDefinition = RowDefinitions.Single(x => x.RowIndex == rowIndex);
        RowDefinitions.Remove(rowDefinition);

        foreach (var row in RowDefinitions.Where(x => x.RowIndex > rowIndex))
            row.RowIndex--;

        var updatedCells = new Dictionary<string, DataCell>();
        foreach (var cell in DataCells.Values)
        {
            if (cell.RowIndex == rowIndex)
                continue;

            if (cell.RowIndex > rowIndex)
                cell.RowIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        DataCells = updatedCells;
        RowsCount = RowDefinitions.Count;
    }

    public void RemoveColumn(int columnIndex)
    {
        Normalize();
        ValidateColumnIndex(columnIndex);

        var columnDefinition = ColumnDefinitions.Single(x => x.ColumnIndex == columnIndex);
        ColumnDefinitions.Remove(columnDefinition);

        foreach (var column in ColumnDefinitions.Where(x => x.ColumnIndex > columnIndex))
            column.ColumnIndex--;

        var updatedCells = new Dictionary<string, DataCell>();
        foreach (var cell in DataCells.Values)
        {
            if (cell.ColumnIndex == columnIndex)
                continue;

            if (cell.ColumnIndex > columnIndex)
                cell.ColumnIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        DataCells = updatedCells;
        ColumnsCount = ColumnDefinitions.Count;
    }

    public DataCell GetCell(int rowIndex, int columnIndex)
    {
        Normalize();
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        return EnsureCellExists(rowIndex, columnIndex);
    }

    public bool TryGetCell(int rowIndex, int columnIndex, out DataCell? cell)
    {
        Normalize();
        cell = null;

        if (rowIndex < 0 || rowIndex >= RowsCount || columnIndex < 0 || columnIndex >= ColumnsCount)
            return false;

        return DataCells.TryGetValue(BuildCellKey(rowIndex, columnIndex), out cell);
    }

    public void Normalize()
    {
        if (string.IsNullOrWhiteSpace(SectionId))
            SectionId = SectionIds.New();

        RowDefinitions ??= new List<RowDefinition>();
        ColumnDefinitions ??= new List<DataColumnDefinition>();
        DataCells ??= new Dictionary<string, DataCell>();

        RowsCount = Math.Max(RowsCount, RowDefinitions.Count);
        ColumnsCount = Math.Max(ColumnsCount, ColumnDefinitions.Count);

        NormalizeRows();
        NormalizeColumns();
        NormalizeCells();
    }

    private void NormalizeRows()
    {
        var existingRows = RowDefinitions
            .Where(x => x.RowIndex >= 0)
            .GroupBy(x => x.RowIndex)
            .ToDictionary(x => x.Key, x => x.First());

        RowDefinitions = Enumerable.Range(0, RowsCount)
            .Select(index =>
            {
                if (existingRows.TryGetValue(index, out var row))
                {
                    row.RowIndex = index;
                    if (row.Height <= 0)
                        row.Height = DefaultRowHeight;

                    return row;
                }

                return new RowDefinition
                {
                    RowIndex = index,
                    Height = DefaultRowHeight
                };
            })
            .ToList();
    }

    private void NormalizeColumns()
    {
        var existingColumns = ColumnDefinitions
            .Where(x => x.ColumnIndex >= 0)
            .GroupBy(x => x.ColumnIndex)
            .ToDictionary(x => x.Key, x => x.First());

        ColumnDefinitions = Enumerable.Range(0, ColumnsCount)
            .Select(index =>
            {
                if (existingColumns.TryGetValue(index, out var column))
                {
                    column.ColumnIndex = index;
                    if (column.Width <= 0)
                        column.Width = DefaultColumnWidth;

                    return column;
                }

                return new DataColumnDefinition
                {
                    ColumnIndex = index,
                    Width = DefaultColumnWidth
                };
            })
            .ToList();
    }

    private void NormalizeCells()
    {
        var normalizedCells = new Dictionary<string, DataCell>();

        foreach (var cell in DataCells.Values)
        {
            if (cell.RowIndex < 0 || cell.RowIndex >= RowsCount || cell.ColumnIndex < 0 || cell.ColumnIndex >= ColumnsCount)
                continue;

            normalizedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        for (var rowIndex = 0; rowIndex < RowsCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
            {
                var key = BuildCellKey(rowIndex, columnIndex);
                if (normalizedCells.ContainsKey(key))
                    continue;

                normalizedCells[key] = new DataCell
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex
                };
            }
        }

        DataCells = normalizedCells;
    }

    private DataCell EnsureCellExists(int rowIndex, int columnIndex)
    {
        var key = BuildCellKey(rowIndex, columnIndex);
        if (DataCells.TryGetValue(key, out var cell))
            return cell;

        cell = new DataCell
        {
            RowIndex = rowIndex,
            ColumnIndex = columnIndex
        };

        DataCells[key] = cell;
        return cell;
    }

    private void ValidateRowIndex(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= RowsCount)
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
    }

    private void ValidateColumnIndex(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= ColumnsCount)
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
    }

    private static string BuildCellKey(int rowIndex, int columnIndex) => $"R{rowIndex}C{columnIndex}";
}
