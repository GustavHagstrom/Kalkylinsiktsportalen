using BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection;

namespace BlazorServer.Modules.Reports.Views.Editing;

public class DataSectionEditor
{
    private const double DefaultRowHeight = 8;
    private const int DefaultColumnWidth = 10;

    public DataSectionEditor(DataSectionDto dataSection)
    {
        DataSection = dataSection ?? throw new ArgumentNullException(nameof(dataSection));
        Normalize();
    }

    public DataSectionDto DataSection { get; }
    public IReadOnlyList<RowDefinition> Rows => DataSection.RowDefinitions;
    public IReadOnlyList<DataColumnDefinition> Columns => DataSection.ColumnDefinitions;
    public int RowsCount => DataSection.RowsCount;
    public int ColumnsCount => DataSection.ColumnsCount;

    public void Initialize(int rowsCount, int columnsCount)
    {
        if (rowsCount < 0)
            throw new ArgumentOutOfRangeException(nameof(rowsCount));

        if (columnsCount < 0)
            throw new ArgumentOutOfRangeException(nameof(columnsCount));

        DataSection.RowDefinitions.Clear();
        DataSection.ColumnDefinitions.Clear();
        DataSection.DataCells.Clear();
        DataSection.RowsCount = 0;
        DataSection.ColumnsCount = 0;

        for (var i = 0; i < rowsCount; i++)
            AddRow();

        for (var i = 0; i < columnsCount; i++)
            AddColumn();
    }

    public RowDefinition AddRow(double height = DefaultRowHeight)
    {
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        var row = new RowDefinition
        {
            RowIndex = RowsCount,
            Height = height
        };

        DataSection.RowDefinitions.Add(row);
        DataSection.RowsCount = DataSection.RowDefinitions.Count;

        for (var columnIndex = 0; columnIndex < ColumnsCount; columnIndex++)
            EnsureCellExists(row.RowIndex, columnIndex);

        return row;
    }

    public DataColumnDefinition AddColumn(int width = DefaultColumnWidth)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        var column = new DataColumnDefinition
        {
            ColumnIndex = ColumnsCount,
            Width = width
        };

        DataSection.ColumnDefinitions.Add(column);
        DataSection.ColumnsCount = DataSection.ColumnDefinitions.Count;

        for (var rowIndex = 0; rowIndex < RowsCount; rowIndex++)
            EnsureCellExists(rowIndex, column.ColumnIndex);

        return column;
    }

    public void RemoveRow(int rowIndex)
    {
        ValidateRowIndex(rowIndex);

        var rowDefinition = DataSection.RowDefinitions.Single(x => x.RowIndex == rowIndex);
        DataSection.RowDefinitions.Remove(rowDefinition);

        foreach (var row in DataSection.RowDefinitions.Where(x => x.RowIndex > rowIndex))
            row.RowIndex--;

        var updatedCells = new Dictionary<string, DataCellDto>();
        foreach (var cell in DataSection.DataCells.Values)
        {
            if (cell.RowIndex == rowIndex)
                continue;

            if (cell.RowIndex > rowIndex)
                cell.RowIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        DataSection.DataCells = updatedCells;
        DataSection.RowsCount = DataSection.RowDefinitions.Count;
    }

    public void RemoveColumn(int columnIndex)
    {
        ValidateColumnIndex(columnIndex);

        var columnDefinition = DataSection.ColumnDefinitions.Single(x => x.ColumnIndex == columnIndex);
        DataSection.ColumnDefinitions.Remove(columnDefinition);

        foreach (var column in DataSection.ColumnDefinitions.Where(x => x.ColumnIndex > columnIndex))
            column.ColumnIndex--;

        var updatedCells = new Dictionary<string, DataCellDto>();
        foreach (var cell in DataSection.DataCells.Values)
        {
            if (cell.ColumnIndex == columnIndex)
                continue;

            if (cell.ColumnIndex > columnIndex)
                cell.ColumnIndex--;

            updatedCells[BuildCellKey(cell.RowIndex, cell.ColumnIndex)] = cell;
        }

        DataSection.DataCells = updatedCells;
        DataSection.ColumnsCount = DataSection.ColumnDefinitions.Count;
    }

    public DataCellDto GetCell(int rowIndex, int columnIndex)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        return EnsureCellExists(rowIndex, columnIndex);
    }

    public bool TryGetCell(int rowIndex, int columnIndex, out DataCellDto? cell)
    {
        cell = null;

        if (rowIndex < 0 || rowIndex >= RowsCount || columnIndex < 0 || columnIndex >= ColumnsCount)
            return false;

        return DataSection.DataCells.TryGetValue(BuildCellKey(rowIndex, columnIndex), out cell);
    }


    private void Normalize()
    {
        DataSection.RowsCount = Math.Max(DataSection.RowsCount, DataSection.RowDefinitions.Count);
        DataSection.ColumnsCount = Math.Max(DataSection.ColumnsCount, DataSection.ColumnDefinitions.Count);

        NormalizeRows();
        NormalizeColumns();
        NormalizeCells();
    }

    private void NormalizeRows()
    {
        var existingRows = DataSection.RowDefinitions
            .Where(x => x.RowIndex >= 0)
            .GroupBy(x => x.RowIndex)
            .ToDictionary(x => x.Key, x => x.First());

        DataSection.RowDefinitions = Enumerable.Range(0, DataSection.RowsCount)
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
        var existingColumns = DataSection.ColumnDefinitions
            .Where(x => x.ColumnIndex >= 0)
            .GroupBy(x => x.ColumnIndex)
            .ToDictionary(x => x.Key, x => x.First());

        DataSection.ColumnDefinitions = Enumerable.Range(0, DataSection.ColumnsCount)
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
        var normalizedCells = new Dictionary<string, DataCellDto>();

        foreach (var cell in DataSection.DataCells.Values)
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

                normalizedCells[key] = new DataCellDto
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex
                };
            }
        }

        DataSection.DataCells = normalizedCells;
    }

    private DataCellDto EnsureCellExists(int rowIndex, int columnIndex)
    {
        var key = BuildCellKey(rowIndex, columnIndex);
        if (DataSection.DataCells.TryGetValue(key, out var cell))
            return cell;

        cell = new DataCellDto
        {
            RowIndex = rowIndex,
            ColumnIndex = columnIndex
        };

        DataSection.DataCells[key] = cell;
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
