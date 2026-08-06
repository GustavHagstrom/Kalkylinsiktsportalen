using BlazorServer.Identity;
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

        var orderedSections = GetSectionHandles()
            .OrderBy(x => x.SectionIndex)
            .ToList();

        for (var index = 0; index < orderedSections.Count; index++)
            orderedSections[index].SetSectionIndex(index);
    }

    private void CollapseSectionIndexesAfter(int removedSectionIndex)
    {
        foreach (var section in GetSectionHandles().Where(x => x.SectionIndex > removedSectionIndex))
            section.SetSectionIndex(section.SectionIndex - 1);
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
