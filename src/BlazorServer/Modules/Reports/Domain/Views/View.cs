using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Reports.Domain.Views;

public class View
{
    private readonly List<IViewSection> _sections = [];

    public ViewId Id { get; private set; } = ViewId.New();
    public OrganizationId OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public PageFormat PageFormat { get; private set; } = PageFormat.A4;
    public bool IsArchived { get; private set; } = false;

    public IReadOnlyList<IViewSection> Sections => _sections;

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
        IReadOnlyList<IViewSection> sections,
        PageFormat? pageFormat = null)
    {
        var view = new View
        {
            Id = id.IsEmpty ? ViewId.New() : id,
            OrganizationId = organizationId,
            Name = string.IsNullOrWhiteSpace(name) ? throw new ReportsDomainException(
                "view.name.required",
                "A view must have a name.") : name.Trim(),
            Description = description ?? string.Empty,
            PageFormat = pageFormat ?? PageFormat.A4,
            IsArchived = isArchived,
        };
        view._sections.AddRange(sections);
        return view;
    }
    public View Duplicate(string name)
    {
        var duplicate = Create(OrganizationId, name, Description);
        duplicate.ChangePageFormat(PageFormat);

        foreach (var section in _sections)
        {
            switch (section)
            {
                case DataSection dataSection:
                    duplicate.AddSection(DuplicateDataSection(dataSection));
                    break;
                case SheetSection sheetSection:
                    duplicate.AddSection(DuplicateSheetSection(sheetSection));
                    break;
            }
            if (section.SectionType == ViewSectionType.Data)
                duplicate.AddSection(DuplicateDataSection(section.DataSection!));
            else
                duplicate.AddSection(DuplicateSheetSection(section.SheetSection!));
        }

        return duplicate;
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

    public void ChangePageFormat(PageFormat pageFormat)
    {
        ArgumentNullException.ThrowIfNull(pageFormat);
        EnsureActive();
        PageFormat = pageFormat;
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void Reactivate()
    {
        IsArchived = false;
    }

    public DataSection AddDataSection()
    {
        var section = DataSection.CreateDefault(_sections.Count);
        AddSection(section);
        return section;
    }

    public SheetSection AddSheetSection()
    {
        var section = new SheetSection();
        AddSection(section);
        return section;
    }
    private void AddSection(IViewSection section)
    {
        EnsureActive();
        
        if (_sections.Any(existing => existing.Id == section.Id))
        {
            throw new ReportsDomainException(
                "view.section_id.duplicate",
                "A view cannot contain duplicate section identities.");
        }

        _sections.Add(section);
    }
    public void RemoveSection(SectionId id)
    {
        EnsureActive();

        var index = _sections.FindIndex(section => section.Id == id);
        if (index < 0)
        {
            throw new ReportsDomainException(
                "view.section.not_found",
                "The section does not belong to this view.");
        }
        _sections.RemoveAt(index);
    }

    public void MoveSection(SectionId id, int targetIndex)
    {
        EnsureActive();

        if (targetIndex < 0 || targetIndex >= _sections.Count)
            throw new ArgumentOutOfRangeException(nameof(targetIndex));

        var currentIndex = _sections.FindIndex(section => section.Id == id);
        if (currentIndex < 0)
        {
            throw new ReportsDomainException(
                "view.section.not_found",
                "The section does not belong to this view.");
        }

        var section = _sections[currentIndex];
        _sections.RemoveAt(currentIndex);
        _sections.Insert(targetIndex, section);
    }


    

    public void ApplyLegacySectionBridge(
        IReadOnlyList<DataSection> dataSections,
        IReadOnlyList<SheetSection> sheetSections)
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

  



    private void ValidateEditedSectionIdentities(
        IReadOnlyList<DataSection> dataSections,
        IReadOnlyList<SheetSection> sheetSections)
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
        IReadOnlyList<DataSection> dataSections,
        IReadOnlyList<SheetSection> sheetSections)
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
        IReadOnlyList<DataSection> dataSections,
        IReadOnlyList<SheetSection> sheetSections)
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

    private static void ValidateUniqueSectionIds(IReadOnlyList<IViewSection> sections)
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

    private static DataSection CloneDataSection(DataSection section)
    {
        return new DataSection.DataSection
        {
            SectionId = section.SectionId,
            SectionIndex = section.SectionIndex,
            RowsCount = section.RowsCount,
            ColumnsCount = section.ColumnsCount,
            RowDefinitions = section.RowDefinitions
                .Select(row => new RowDefinition
                {
                    RowIndex = row.RowIndex,
                    Height = row.Height
                })
                .ToList(),
            ColumnDefinitions = section.ColumnDefinitions
                .Select(column => new DataColumnDefinition
                {
                    ColumnIndex = column.ColumnIndex,
                    Width = column.Width
                })
                .ToList(),
            DataCells = section.DataCells.ToDictionary(
                cell => cell.Key,
                cell => CloneFormatting(new DataCell
                {
                    RowIndex = cell.Value.RowIndex,
                    ColumnIndex = cell.Value.ColumnIndex,
                    RawValue = cell.Value.RawValue
                }, cell.Value))
        };
    }

    private static SheetSection CloneSheetSection(SheetSection section)
    {
        return new SheetSection.SheetSection
        {
            SectionId = section.SectionId,
            SectionIndex = section.SectionIndex,
            Content = section.Content,
            EnableChanges = section.EnableChanges,
            SheetCells = section.SheetCells.ToDictionary(
                cell => cell.Key,
                cell => CloneFormatting(new SheetCell
                {
                    RowType = cell.Value.RowType,
                    ColumnType = cell.Value.ColumnType,
                    HideValue = cell.Value.HideValue
                }, cell.Value))
        };
    }

    private static DataSection DuplicateDataSection(DataSection section)
    {
        var duplicate = CloneDataSection(section);
        duplicate.SectionId = SectionId.New();
        return duplicate;
    }

    private static SheetSection DuplicateSheetSection(.SheetSection section)
    {
        var duplicate = CloneSheetSection(section);
        duplicate.SectionId = SectionId.New();
        return duplicate;
    }

    private static TCell CloneFormatting<TCell>(TCell target, IReportFormattedCell source)
        where TCell : IReportFormattedCell
    {
        target.CellLayout = CloneCellLayout(source.CellLayout);
        target.TextStyle = CloneTextStyle(source.TextStyle);
        target.ValueFormat = CloneValueFormat(source.ValueFormat);
        target.TextFormat = CloneTextFormat(source.TextFormat);
        target.CellFormat = CloneCellFormat(source.CellFormat);
        return target;
    }

    private static TextFormat? CloneTextFormat(TextFormat? format)
        => format is null
            ? null
            : new TextFormat
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

    private static CellFormat? CloneCellFormat(CellFormat? format)
        => format is null
            ? null
            : new CellFormat
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

    private static CellLayout? CloneCellLayout(CellLayout? layout)
        => layout is null
            ? null
            : new CellLayout
            {
                BackgroundColor = layout.BackgroundColor,
                PaddingLeftMm = layout.PaddingLeftMm,
                PaddingRightMm = layout.PaddingRightMm,
                PaddingTopMm = layout.PaddingTopMm,
                PaddingBottomMm = layout.PaddingBottomMm,
                LeftBorder = CloneCellBorder(layout.LeftBorder),
                RightBorder = CloneCellBorder(layout.RightBorder),
                TopBorder = CloneCellBorder(layout.TopBorder),
                BottomBorder = CloneCellBorder(layout.BottomBorder)
            };

    private static CellBorder? CloneCellBorder(CellBorder? border)
        => border is null
            ? null
            : new CellBorder
            {
                WidthPt = border.WidthPt,
                Style = border.Style,
                Color = border.Color
            };

    private static TextStyle? CloneTextStyle(TextStyle? style)
        => style is null
            ? null
            : new TextStyle
            {
                Font = style.Font,
                TextColor = style.TextColor,
                FontSizePt = style.FontSizePt,
                IsBold = style.IsBold,
                IsItalic = style.IsItalic,
                IsUnderline = style.IsUnderline,
                HorizontalAlignment = style.HorizontalAlignment,
                VerticalAlignment = style.VerticalAlignment
            };

    private static ValueFormat? CloneValueFormat(ValueFormat? valueFormat)
        => valueFormat is null
            ? null
            : new ValueFormat
            {
                Kind = valueFormat.Kind,
                DecimalCount = valueFormat.DecimalCount,
                UseThousandsSeparator = valueFormat.UseThousandsSeparator
            };
}
