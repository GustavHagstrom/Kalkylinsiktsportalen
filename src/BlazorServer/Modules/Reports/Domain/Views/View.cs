using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Public;
using DataSectionModel = BlazorServer.Modules.Reports.Domain.Views.DataSection.DataSection;
using SheetSectionModel = BlazorServer.Modules.Reports.Domain.Views.SheetSection.SheetSection;

namespace BlazorServer.Modules.Reports.Domain.Views;

public class View
{
    public ViewId Id { get; init; } = ViewId.New();
    public OrganizationId OrganizationId { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool EnableChanges { get; set; } = false;
    public bool IsArchived { get; set; } = false;
    public List<DataSectionModel> DataSections { get; set; } = new();
    public List<SheetSectionModel> SheetSections { get; set; } = new();

    public int SectionCount => DataSections.Count + SheetSections.Count;
    public IReadOnlyList<ViewSection> Sections => GetSections();

    public static View Create(
        OrganizationId organizationId,
        string name,
        string description = "")
    {
        var view = new View
        {
            OrganizationId = organizationId
        };

        view.UpdateMetadata(name, description);
        return view;
    }

    public void UpdateMetadata(string name, string description)
    {
        EnsureActive();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ReportsDomainException(
                "view.name.required",
                "View name is required.");
        }

        Name = name.Trim();
        Description = description.Trim();
    }

    public void EnableChangeEdits()
    {
        EnsureActive();
        EnableChanges = true;
    }

    public void DisableChangeEdits()
    {
        EnsureActive();
        EnableChanges = false;
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void Reactivate()
    {
        IsArchived = false;
    }

    public ViewSection AddSection(ViewSectionType sectionType)
    {
        return sectionType switch
        {
            ViewSectionType.Data => ViewSection.From(AddDataSection()),
            ViewSectionType.Sheet => ViewSection.From(AddSheetSection()),
            _ => throw new ArgumentOutOfRangeException(nameof(sectionType), sectionType, "Unknown view section type.")
        };
    }

    public DataSectionModel AddDataSection()
    {
        EnsureActive();
        Normalize();

        var section = DataSectionModel.CreateDefault(SectionCount);
        DataSections.Add(section);
        return section;
    }

    public SheetSectionModel AddSheetSection()
    {
        EnsureActive();
        Normalize();

        var section = new SheetSectionModel
        {
            SectionIndex = SectionCount
        };

        SheetSections.Add(section);
        return section;
    }

    public void RemoveDataSection(DataSectionModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        EnsureActive();
        Normalize();

        if (!DataSections.Remove(section))
            throw new InvalidOperationException("The data section does not belong to this view.");

        CollapseSectionIndexesAfter(section.SectionIndex);
    }

    public void RemoveSheetSection(SheetSectionModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        EnsureActive();
        Normalize();

        if (!SheetSections.Remove(section))
            throw new InvalidOperationException("The sheet section does not belong to this view.");

        CollapseSectionIndexesAfter(section.SectionIndex);
    }

    public void RemoveSection(string sectionId)
    {
        EnsureActive();

        if (string.IsNullOrWhiteSpace(sectionId))
        {
            throw new ReportsDomainException(
                "view.section_id.required",
                "Section id is required.");
        }

        Normalize();

        var dataSection = DataSections.SingleOrDefault(section => section.SectionId == sectionId);
        if (dataSection is not null)
        {
            RemoveDataSection(dataSection);
            return;
        }

        var sheetSection = SheetSections.SingleOrDefault(section => section.SectionId == sectionId);
        if (sheetSection is not null)
        {
            RemoveSheetSection(sheetSection);
            return;
        }

        throw new ReportsDomainException(
            "view.section.not_found",
            "The section does not belong to this view.");
    }

    public void MoveSection(string sectionId, int newIndex)
    {
        EnsureActive();

        if (string.IsNullOrWhiteSpace(sectionId))
        {
            throw new ReportsDomainException(
                "view.section_id.required",
                "Section id is required.");
        }

        Normalize();

        var sections = Sections.ToList();
        if (newIndex < 0 || newIndex >= sections.Count)
            throw new ArgumentOutOfRangeException(nameof(newIndex));

        var movingSection = sections.SingleOrDefault(section => section.SectionId == sectionId);
        if (movingSection is null)
        {
            throw new ReportsDomainException(
                "view.section.not_found",
                "The section does not belong to this view.");
        }

        sections.Remove(movingSection);
        sections.Insert(newIndex, movingSection);

        for (var index = 0; index < sections.Count; index++)
            sections[index].SetSectionIndex(index);
    }

    public void ApplyEditedSections(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        ArgumentNullException.ThrowIfNull(dataSections);
        ArgumentNullException.ThrowIfNull(sheetSections);

        EnsureActive();
        Normalize();
        ValidateEditedSectionIdentities(dataSections, sheetSections);
        ValidateExistingSectionTypes(dataSections, sheetSections);
        ValidateEditedSectionOrder(dataSections, sheetSections);

        DataSections = dataSections.ToList();
        SheetSections = sheetSections.ToList();
        Normalize();
    }

    public View Duplicate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ReportsDomainException(
                "view.name.required",
                "View name is required.");
        }

        Normalize();

        var copy = new View
        {
            OrganizationId = OrganizationId,
            Name = name.Trim(),
            Description = Description,
            EnableChanges = EnableChanges,
            DataSections = DataSections.Select(CloneDataSection).ToList(),
            SheetSections = SheetSections.Select(CloneSheetSection).ToList()
        };

        copy.Normalize();
        return copy;
    }

    public void Normalize()
    {
        DataSections ??= new List<DataSectionModel>();
        SheetSections ??= new List<SheetSectionModel>();

        EnsureSectionIdentities();
        ValidateUniqueSectionIdentities();

        var orderedSections = GetSectionHandles()
            .OrderBy(x => x.SectionIndex)
            .ToList();

        for (var index = 0; index < orderedSections.Count; index++)
            orderedSections[index].SetSectionIndex(index);
    }

    public void EnsureSectionIdentities()
    {
        DataSections ??= new List<DataSectionModel>();
        SheetSections ??= new List<SheetSectionModel>();

        foreach (var section in DataSections)
        {
            if (string.IsNullOrWhiteSpace(section.SectionId))
                section.SectionId = SectionIds.New();
        }

        foreach (var section in SheetSections)
        {
            if (string.IsNullOrWhiteSpace(section.SectionId))
                section.SectionId = SectionIds.New();
        }
    }

    private void CollapseSectionIndexesAfter(int removedSectionIndex)
    {
        foreach (var section in GetSectionHandles().Where(x => x.SectionIndex > removedSectionIndex))
            section.SetSectionIndex(section.SectionIndex - 1);
    }

    private IReadOnlyList<ViewSection> GetSections()
    {
        Normalize();

        return DataSections.Select(ViewSection.From)
            .Concat(SheetSections.Select(ViewSection.From))
            .OrderBy(section => section.SectionIndex)
            .ToList();
    }

    private void EnsureActive()
    {
        if (!IsArchived)
            return;

        throw new ReportsDomainException(
            "view.archived",
            "Archived views cannot be edited.");
    }

    private void ValidateUniqueSectionIdentities()
    {
        var sectionIds = DataSections
            .Select(section => section.SectionId)
            .Concat(SheetSections.Select(section => section.SectionId))
            .ToList();

        if (sectionIds.Distinct(StringComparer.Ordinal).Count() == sectionIds.Count)
            return;

        throw new ReportsDomainException(
            "view.section_id.duplicate",
            "View sections must not contain duplicate section identities.");
    }

    private void ValidateEditedSectionIdentities(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        var sectionIds = dataSections
            .Select(section => section.SectionId)
            .Concat(sheetSections.Select(section => section.SectionId))
            .ToList();

        if (sectionIds.Any(string.IsNullOrWhiteSpace))
        {
            throw new ReportsDomainException(
                "view.section_id.required",
                "Edited view sections must have stable section identities.");
        }

        if (sectionIds.Distinct(StringComparer.Ordinal).Count() != sectionIds.Count)
        {
            throw new ReportsDomainException(
                "view.section_id.duplicate",
                "Edited view sections must not contain duplicate section identities.");
        }
    }

    private void ValidateExistingSectionTypes(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        var existingDataSectionIds = DataSections
            .Select(section => section.SectionId)
            .ToHashSet(StringComparer.Ordinal);

        var existingSheetSectionIds = SheetSections
            .Select(section => section.SectionId)
            .ToHashSet(StringComparer.Ordinal);

        if (dataSections.Any(section => existingSheetSectionIds.Contains(section.SectionId))
            || sheetSections.Any(section => existingDataSectionIds.Contains(section.SectionId)))
        {
            throw new ReportsDomainException(
                "view.section_type.changed",
                "Edited view sections must keep the same section type for existing section identities.");
        }
    }

    private void ValidateEditedSectionOrder(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        var sectionIndexes = dataSections
            .Select(section => section.SectionIndex)
            .Concat(sheetSections.Select(section => section.SectionIndex))
            .Order()
            .ToList();

        for (var index = 0; index < sectionIndexes.Count; index++)
        {
            if (sectionIndexes[index] == index)
                continue;

            throw new ReportsDomainException(
                "view.section_order.invalid",
                "Edited view sections must use contiguous section indexes.");
        }
    }

    private IEnumerable<SectionHandle> GetSectionHandles()
    {
        foreach (var section in DataSections)
            yield return new SectionHandle(section.SectionIndex, value => section.SectionIndex = value);

        foreach (var section in SheetSections)
            yield return new SectionHandle(section.SectionIndex, value => section.SectionIndex = value);
    }

    private static int GetSectionIndex(object section)
    {
        return section switch
        {
            DataSectionModel dataSection => dataSection.SectionIndex,
            SheetSectionModel sheetSection => sheetSection.SectionIndex,
            _ => throw new ArgumentException("Unknown section type.", nameof(section))
        };
    }

    private static DataSectionModel CloneDataSection(DataSectionModel section)
    {
        return new DataSectionModel
        {
            SectionId = SectionIds.New(),
            SectionIndex = section.SectionIndex,
            RowsCount = section.RowsCount,
            ColumnsCount = section.ColumnsCount,
            RowDefinitions = section.RowDefinitions.Select(row => new DataSection.RowDefinition
            {
                RowIndex = row.RowIndex,
                Height = row.Height
            }).ToList(),
            ColumnDefinitions = section.ColumnDefinitions.Select(column => new DataSection.DataColumnDefinition
            {
                ColumnIndex = column.ColumnIndex,
                Width = column.Width
            }).ToList(),
            DataCells = section.DataCells.ToDictionary(
                cell => cell.Key,
                cell => new DataSection.DataCell
                {
                    RowIndex = cell.Value.RowIndex,
                    ColumnIndex = cell.Value.ColumnIndex,
                    RawValue = cell.Value.RawValue,
                    TextFormat = CloneTextFormat(cell.Value.TextFormat),
                    CellFormat = CloneCellFormat(cell.Value.CellFormat)
                })
        };
    }

    private static SheetSectionModel CloneSheetSection(SheetSectionModel section)
    {
        return new SheetSectionModel
        {
            SectionId = SectionIds.New(),
            SectionIndex = section.SectionIndex,
            Content = section.Content,
            SheetCells = section.SheetCells.ToDictionary(
                cell => cell.Key,
                cell => new SheetSection.SheetCell
                {
                    RowType = cell.Value.RowType,
                    ColumnType = cell.Value.ColumnType,
                    HideValue = cell.Value.HideValue,
                    TextFormat = CloneTextFormat(cell.Value.TextFormat),
                    CellFormat = CloneCellFormat(cell.Value.CellFormat)
                })
        };
    }

    private static TextFormat CloneTextFormat(TextFormat format)
    {
        return new TextFormat
        {
            FontFamily = format.FontFamily,
            Color = format.Color,
            FontSize = format.FontSize,
            IsBold = format.IsBold,
            IsItalic = format.IsItalic,
            IsUnderline = format.IsUnderline,
            HorizontalAlign = format.HorizontalAlign,
            VerticalAlign = format.VerticalAlign,
            Type = format.Type,
            DayTimeFormat = format.DayTimeFormat,
            DecimalCount = format.DecimalCount,
            HasThoasandsSeparator = format.HasThoasandsSeparator
        };
    }

    private static CellFormat CloneCellFormat(CellFormat format)
    {
        return new CellFormat
        {
            LeftBorder = CloneBorderFormat(format.LeftBorder),
            RightBorder = CloneBorderFormat(format.RightBorder),
            TopBorder = CloneBorderFormat(format.TopBorder),
            BottomBorder = CloneBorderFormat(format.BottomBorder),
            BackgroundColor = format.BackgroundColor,
            PaddingLeft = format.PaddingLeft,
            PaddingRight = format.PaddingRight,
            PaddingTop = format.PaddingTop,
            PaddingBottom = format.PaddingBottom
        };
    }

    private static BorderFormat CloneBorderFormat(BorderFormat format)
    {
        return new BorderFormat
        {
            IsBorder = format.IsBorder,
            BorderStyle = format.BorderStyle,
            BorderColor = format.BorderColor,
            BorderWidth = format.BorderWidth
        };
    }

    private sealed record SectionHandle(int SectionIndex, Action<int> SetSectionIndex);
}

public enum ViewSectionType
{
    Data,
    Sheet
}

public sealed class ViewSection
{
    private readonly Action<int> _setSectionIndex;

    private ViewSection(
        string sectionId,
        int sectionIndex,
        ViewSectionType sectionType,
        object section,
        Action<int> setSectionIndex)
    {
        SectionId = sectionId;
        SectionIndex = sectionIndex;
        SectionType = sectionType;
        Section = section;
        _setSectionIndex = setSectionIndex;
    }

    public string SectionId { get; }
    public int SectionIndex { get; private set; }
    public ViewSectionType SectionType { get; }
    public object Section { get; }

    public static ViewSection From(DataSectionModel section)
        => new(
            section.SectionId,
            section.SectionIndex,
            ViewSectionType.Data,
            section,
            value => section.SectionIndex = value);

    public static ViewSection From(SheetSectionModel section)
        => new(
            section.SectionId,
            section.SectionIndex,
            ViewSectionType.Sheet,
            section,
            value => section.SectionIndex = value);

    internal void SetSectionIndex(int sectionIndex)
    {
        SectionIndex = sectionIndex;
        _setSectionIndex(sectionIndex);
    }
}
