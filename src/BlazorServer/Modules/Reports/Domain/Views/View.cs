using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Public;
using DataCellModel = BlazorServer.Modules.Reports.Domain.Views.DataSection.DataCell;
using DataColumnDefinitionModel = BlazorServer.Modules.Reports.Domain.Views.DataSection.DataColumnDefinition;
using DataSectionModel = BlazorServer.Modules.Reports.Domain.Views.DataSection.DataSection;
using RowDefinitionModel = BlazorServer.Modules.Reports.Domain.Views.DataSection.RowDefinition;
using SheetCellModel = BlazorServer.Modules.Reports.Domain.Views.SheetSection.SheetCell;
using SheetSectionModel = BlazorServer.Modules.Reports.Domain.Views.SheetSection.SheetSection;

namespace BlazorServer.Modules.Reports.Domain.Views;

public class View
{
    private readonly List<ViewSection> _sections = [];

    public ViewId Id { get; private set; } = ViewId.New();
    public OrganizationId OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool EnableChanges { get; private set; } = false;
    public bool IsArchived { get; private set; } = false;

    public IReadOnlyList<ViewSection> Sections => _sections;

    public IReadOnlyList<DataSectionModel> LegacyDataSections => OrderedDataSections().Select(CloneDataSection).ToList();
    public IReadOnlyList<SheetSectionModel> LegacySheetSections => OrderedSheetSections().Select(CloneSheetSection).ToList();
    internal IReadOnlyList<DataSectionModel> MutableLegacyDataSections => OrderedDataSections();
    internal IReadOnlyList<SheetSectionModel> MutableLegacySheetSections => OrderedSheetSections();

    public int SectionCount => _sections.Count;

    public static View Create(OrganizationId organizationId, string name, string description = "")
    {
        var view = new View
        {
            Id = ViewId.New(),
            OrganizationId = organizationId
        };

        view.Rename(name);
        view.UpdateDescription(description);
        return view;
    }

    public static View Reconstitute(
        ViewId id,
        OrganizationId organizationId,
        string name,
        string description,
        bool enableChanges,
        bool isArchived,
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        var view = new View
        {
            Id = id.IsEmpty ? ViewId.New() : id,
            OrganizationId = organizationId,
            Name = string.IsNullOrWhiteSpace(name) ? throw new ReportsDomainException(
                "view.name.required",
                "A view must have a name.") : name.Trim(),
            Description = description ?? string.Empty,
            EnableChanges = enableChanges,
            IsArchived = isArchived
        };

        view.ReplaceLegacySections(dataSections, sheetSections);
        return view;
    }

    public void Rename(string name)
    {
        EnsureActive();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ReportsDomainException(
                "view.name.required",
                "A view must have a name.");
        }

        Name = name.Trim();
    }

    public void UpdateDescription(string description)
    {
        EnsureActive();
        Description = description ?? string.Empty;
    }

    public void SetEnableChanges(bool enableChanges)
    {
        EnsureActive();
        EnableChanges = enableChanges;
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void Reactivate()
    {
        IsArchived = false;
    }

    public DataSectionModel AddDataSection()
    {
        var section = DataSectionModel.CreateDefault(_sections.Count);
        AddSection(section);
        return section;
    }

    public SheetSectionModel AddSheetSection()
    {
        var section = new SheetSectionModel();
        AddSection(section);
        return section;
    }

    public void AddSection(DataSectionModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        AddSection(ViewSection.FromData(section));
    }

    public void AddSection(SheetSectionModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        AddSection(ViewSection.FromSheet(section));
    }

    public void RemoveSection(string sectionId)
    {
        EnsureActive();

        var index = _sections.FindIndex(section => section.SectionId == sectionId);
        if (index < 0)
        {
            throw new ReportsDomainException(
                "view.section.not_found",
                "The section does not belong to this view.");
        }

        _sections.RemoveAt(index);
        SynchronizeLegacySectionIndexes();
    }

    public void MoveSection(string sectionId, int targetIndex)
    {
        EnsureActive();

        if (targetIndex < 0 || targetIndex >= _sections.Count)
            throw new ArgumentOutOfRangeException(nameof(targetIndex));

        var currentIndex = _sections.FindIndex(section => section.SectionId == sectionId);
        if (currentIndex < 0)
        {
            throw new ReportsDomainException(
                "view.section.not_found",
                "The section does not belong to this view.");
        }

        var section = _sections[currentIndex];
        _sections.RemoveAt(currentIndex);
        _sections.Insert(targetIndex, section);
        SynchronizeLegacySectionIndexes();
    }

    public void RemoveDataSection(DataSectionModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        RemoveSection(section.SectionId);
    }

    public void RemoveSheetSection(SheetSectionModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        RemoveSection(section.SectionId);
    }

    public View Duplicate(string name)
    {
        var duplicate = Create(OrganizationId, name, Description);
        duplicate.SetEnableChanges(EnableChanges);

        foreach (var section in _sections)
        {
            if (section.SectionType == ViewSectionType.Data)
                duplicate.AddSection(DuplicateDataSection(section.DataSection!));
            else
                duplicate.AddSection(DuplicateSheetSection(section.SheetSection!));
        }

        return duplicate;
    }

    public void ApplyLegacySectionBridge(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        ArgumentNullException.ThrowIfNull(dataSections);
        ArgumentNullException.ThrowIfNull(sheetSections);

        EnsureActive();
        ValidateEditedSectionIdentities(dataSections, sheetSections);
        ValidateExistingSectionTypes(dataSections, sheetSections);
        ValidateEditedSectionOrder(dataSections, sheetSections);

        ReplaceLegacySections(dataSections, sheetSections);
    }

    internal IEnumerable<object> GetSectionsInOrder()
        => Sections.Select(section => section.Section);

    private void EnsureSectionIdentities()
    {
        foreach (var section in _sections)
            section.EnsureSectionId();

        SynchronizeLegacySectionIndexes();
    }

    private void AddSection(ViewSection section)
    {
        EnsureActive();
        section.EnsureSectionId();

        if (_sections.Any(existing => existing.SectionId == section.SectionId))
        {
            throw new ReportsDomainException(
                "view.section_id.duplicate",
                "A view cannot contain duplicate section identities.");
        }

        _sections.Add(section);
        SynchronizeLegacySectionIndexes();
    }

    private void ReplaceLegacySections(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        EnsureLegacySectionIdentities(dataSections, sheetSections);

        var sections = dataSections
            .Select(ViewSection.FromData)
            .Concat(sheetSections.Select(ViewSection.FromSheet))
            .OrderBy(section => section.SectionIndex)
            .ToList();

        foreach (var section in sections)
            section.EnsureSectionId();

        ValidateUniqueSectionIds(sections);

        _sections.Clear();
        _sections.AddRange(sections);
        SynchronizeLegacySectionIndexes();
    }

    private IReadOnlyList<DataSectionModel> OrderedDataSections()
    {
        SynchronizeLegacySectionIndexes();
        return _sections
            .Where(section => section.SectionType == ViewSectionType.Data)
            .Select(section => section.DataSection!)
            .ToList();
    }

    private IReadOnlyList<SheetSectionModel> OrderedSheetSections()
    {
        SynchronizeLegacySectionIndexes();
        return _sections
            .Where(section => section.SectionType == ViewSectionType.Sheet)
            .Select(section => section.SheetSection!)
            .ToList();
    }

    private static void EnsureLegacySectionIdentities(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        foreach (var section in dataSections)
        {
            if (string.IsNullOrWhiteSpace(section.SectionId))
                section.SectionId = SectionIds.New();
        }

        foreach (var section in sheetSections)
        {
            if (string.IsNullOrWhiteSpace(section.SectionId))
                section.SectionId = SectionIds.New();
        }
    }

    private void SynchronizeLegacySectionIndexes()
    {
        for (var index = 0; index < _sections.Count; index++)
        {
            _sections[index].SynchronizeSectionIdentity();
            _sections[index].SetSectionIndex(index);
        }
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
        var existingDataSectionIds = _sections
            .Where(section => section.SectionType == ViewSectionType.Data)
            .Select(section => section.SectionId)
            .ToHashSet(StringComparer.Ordinal);

        var existingSheetSectionIds = _sections
            .Where(section => section.SectionType == ViewSectionType.Sheet)
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

    private static void ValidateEditedSectionOrder(
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

    private static void ValidateUniqueSectionIds(IReadOnlyList<ViewSection> sections)
    {
        if (sections.Select(section => section.SectionId).Distinct(StringComparer.Ordinal).Count() != sections.Count)
        {
            throw new ReportsDomainException(
                "view.section_id.duplicate",
                "A view cannot contain duplicate section identities.");
        }
    }

    private void EnsureActive()
    {
        if (IsArchived)
        {
            throw new ReportsDomainException(
                "view.archived",
                "Archived views must be reactivated or duplicated before they can change.");
        }
    }

    private static DataSectionModel CloneDataSection(DataSectionModel section)
    {
        return new DataSectionModel
        {
            SectionId = section.SectionId,
            SectionIndex = section.SectionIndex,
            RowsCount = section.RowsCount,
            ColumnsCount = section.ColumnsCount,
            RowDefinitions = section.RowDefinitions
                .Select(row => new RowDefinitionModel
                {
                    RowIndex = row.RowIndex,
                    Height = row.Height
                })
                .ToList(),
            ColumnDefinitions = section.ColumnDefinitions
                .Select(column => new DataColumnDefinitionModel
                {
                    ColumnIndex = column.ColumnIndex,
                    Width = column.Width
                })
                .ToList(),
            DataCells = section.DataCells.ToDictionary(
                cell => cell.Key,
                cell => new DataCellModel
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
            SectionId = section.SectionId,
            SectionIndex = section.SectionIndex,
            Content = section.Content,
            SheetCells = section.SheetCells.ToDictionary(
                cell => cell.Key,
                cell => new SheetCellModel
                {
                    RowType = cell.Value.RowType,
                    ColumnType = cell.Value.ColumnType,
                    HideValue = cell.Value.HideValue,
                    TextFormat = CloneTextFormat(cell.Value.TextFormat),
                    CellFormat = CloneCellFormat(cell.Value.CellFormat)
                })
        };
    }

    private static DataSectionModel DuplicateDataSection(DataSectionModel section)
    {
        var duplicate = CloneDataSection(section);
        duplicate.SectionId = SectionIds.New();
        return duplicate;
    }

    private static SheetSectionModel DuplicateSheetSection(SheetSectionModel section)
    {
        var duplicate = CloneSheetSection(section);
        duplicate.SectionId = SectionIds.New();
        return duplicate;
    }

    private static TextFormat CloneTextFormat(TextFormat format)
        => new()
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

    private static CellFormat CloneCellFormat(CellFormat format)
        => new()
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

    private static BorderFormat CloneBorderFormat(BorderFormat format)
        => new()
        {
            IsBorder = format.IsBorder,
            BorderStyle = format.BorderStyle,
            BorderColor = format.BorderColor,
            BorderWidth = format.BorderWidth
        };
}

public sealed class ViewSection
{
    private string _sectionId = string.Empty;
    private int _sectionIndex;

    private ViewSection(DataSectionModel? dataSection, SheetSectionModel? sheetSection)
    {
        DataSection = dataSection;
        SheetSection = sheetSection;
        _sectionId = SectionType == ViewSectionType.Data ? dataSection!.SectionId : sheetSection!.SectionId;
        _sectionIndex = SectionType == ViewSectionType.Data ? dataSection!.SectionIndex : sheetSection!.SectionIndex;
    }

    public ViewSectionType SectionType => DataSection is not null ? ViewSectionType.Data : ViewSectionType.Sheet;
    public string SectionId => _sectionId;
    public int SectionIndex => _sectionIndex;
    internal object Section => SectionType == ViewSectionType.Data ? DataSection! : SheetSection!;
    internal DataSectionModel? DataSection { get; }
    internal SheetSectionModel? SheetSection { get; }

    public static ViewSection FromData(DataSectionModel section)
        => new(section, null);

    public static ViewSection FromSheet(SheetSectionModel section)
        => new(null, section);

    public void EnsureSectionId()
    {
        if (SectionType == ViewSectionType.Data)
        {
            if (string.IsNullOrWhiteSpace(DataSection!.SectionId))
                DataSection.SectionId = SectionIds.New();

            if (string.IsNullOrWhiteSpace(_sectionId))
                _sectionId = DataSection.SectionId;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(SheetSection!.SectionId))
                SheetSection.SectionId = SectionIds.New();

            if (string.IsNullOrWhiteSpace(_sectionId))
                _sectionId = SheetSection.SectionId;
        }

        SynchronizeSectionIdentity();
    }

    public void SynchronizeSectionIdentity()
    {
        if (SectionType == ViewSectionType.Data)
            DataSection!.SectionId = _sectionId;
        else
            SheetSection!.SectionId = _sectionId;
    }

    public void SetSectionIndex(int sectionIndex)
    {
        _sectionIndex = sectionIndex;

        if (SectionType == ViewSectionType.Data)
            DataSection!.SectionIndex = sectionIndex;
        else
            SheetSection!.SectionIndex = sectionIndex;
    }
}

public enum ViewSectionType
{
    Data,
    Sheet
}
