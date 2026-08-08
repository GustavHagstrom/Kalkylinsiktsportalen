using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Public;
using DataSectionModel = BlazorServer.Modules.Reports.Domain.Views.DataSection.DataSection;
using SheetSectionModel = BlazorServer.Modules.Reports.Domain.Views.SheetSection.SheetSection;

namespace BlazorServer.Modules.Reports.Domain.Views;

public class View
{
    public ViewId Id { get; set; } = ViewId.New();
    public OrganizationId OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DataSectionModel> DataSections { get; set; } = new();
    public List<SheetSectionModel> SheetSections { get; set; } = new();

    public int SectionCount => DataSections.Count + SheetSections.Count;

    public DataSectionModel AddDataSection()
    {
        Normalize();

        var section = DataSectionModel.CreateDefault(SectionCount);
        DataSections.Add(section);
        return section;
    }

    public SheetSectionModel AddSheetSection()
    {
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
        Normalize();

        if (!DataSections.Remove(section))
            throw new InvalidOperationException("The data section does not belong to this view.");

        CollapseSectionIndexesAfter(section.SectionIndex);
    }

    public void RemoveSheetSection(SheetSectionModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        Normalize();

        if (!SheetSections.Remove(section))
            throw new InvalidOperationException("The sheet section does not belong to this view.");

        CollapseSectionIndexesAfter(section.SectionIndex);
    }

    public void ApplyEditedSections(
        IReadOnlyList<DataSectionModel> dataSections,
        IReadOnlyList<SheetSectionModel> sheetSections)
    {
        ArgumentNullException.ThrowIfNull(dataSections);
        ArgumentNullException.ThrowIfNull(sheetSections);

        Normalize();
        ValidateEditedSectionIdentities(dataSections, sheetSections);
        ValidateExistingSectionTypes(dataSections, sheetSections);
        ValidateEditedSectionOrder(dataSections, sheetSections);

        DataSections = dataSections.ToList();
        SheetSections = sheetSections.ToList();
        Normalize();
    }

    public IEnumerable<object> GetSectionsInOrder()
    {
        Normalize();

        return DataSections.Cast<object>()
            .Concat(SheetSections)
            .OrderBy(GetSectionIndex);
    }

    public void Normalize()
    {
        DataSections ??= new List<DataSectionModel>();
        SheetSections ??= new List<SheetSectionModel>();

        EnsureSectionIdentities();

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

    private sealed record SectionHandle(int SectionIndex, Action<int> SetSectionIndex);
}
