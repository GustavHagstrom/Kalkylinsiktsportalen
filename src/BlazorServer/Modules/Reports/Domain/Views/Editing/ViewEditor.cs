using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;

namespace BlazorServer.Modules.Reports.Domain.Views.Editing;

public class ViewEditor
{
    public ViewEditor(ViewDto view)
    {
        View = view ?? throw new ArgumentNullException(nameof(view));
        Normalize();
    }

    public ViewDto View { get; }
    public IReadOnlyList<DataSectionDto> DataSections => View.DataSections;
    public IReadOnlyList<SheetSectionDto> SheetSections => View.SheetSections;
    public IReadOnlyList<PageBreakSectionDto> PageBreakSections => View.PageBreakSections;
    public int SectionCount => View.DataSections.Count + View.SheetSections.Count + View.PageBreakSections.Count;

    public DataSectionDto AddDataSection()
    {
        var editor = new DataSectionEditor(new DataSectionDto
        {
            SectionIndex = SectionCount
        });
        editor.AddColumn();
        editor.AddColumn();
        editor.AddColumn();
        editor.AddRow();
        editor.AddRow();
        editor.AddRow();

        View.DataSections.Add(editor.DataSection);
        return editor.DataSection;
    }

    public SheetSectionDto AddSheetSection()
    {
        var section = new SheetSectionDto
        {
            SectionIndex = SectionCount
        };

        View.SheetSections.Add(section);
        return section;
    }

    public PageBreakSectionDto AddPageBreakSection()
    {
        var section = new PageBreakSectionDto
        {
            SectionIndex = SectionCount
        };

        View.PageBreakSections.Add(section);
        return section;
    }

    public void RemoveDataSection(DataSectionDto section)
    {
        ArgumentNullException.ThrowIfNull(section);

        if (!View.DataSections.Remove(section))
            throw new InvalidOperationException("The data section does not belong to this view.");

        CollapseSectionIndexesAfter(section.SectionIndex);
    }

    public void RemoveSheetSection(SheetSectionDto section)
    {
        ArgumentNullException.ThrowIfNull(section);

        if (!View.SheetSections.Remove(section))
            throw new InvalidOperationException("The sheet section does not belong to this view.");

        CollapseSectionIndexesAfter(section.SectionIndex);
    }

    public void RemovePageBreakSection(PageBreakSectionDto section)
    {
        ArgumentNullException.ThrowIfNull(section);

        if (!View.PageBreakSections.Remove(section))
            throw new InvalidOperationException("The page break section does not belong to this view.");

        CollapseSectionIndexesAfter(section.SectionIndex);
    }

    public IEnumerable<object> GetSectionsInOrder()
    {
        return View.DataSections.Cast<object>()
            .Concat(View.SheetSections)
            .Concat(View.PageBreakSections)
            .OrderBy(GetSectionIndex);
    }

    private void Normalize()
    {
        View.DataSections ??= new List<DataSectionDto>();
        View.SheetSections ??= new List<SheetSectionDto>();
        View.PageBreakSections ??= new List<PageBreakSectionDto>();

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
        foreach (var section in View.DataSections)
            yield return new SectionHandle(section.SectionIndex, value => section.SectionIndex = value);

        foreach (var section in View.SheetSections)
            yield return new SectionHandle(section.SectionIndex, value => section.SectionIndex = value);

        foreach (var section in View.PageBreakSections)
            yield return new SectionHandle(section.SectionIndex, value => section.SectionIndex = value);
    }

    private static int GetSectionIndex(object section)
    {
        return section switch
        {
            DataSectionDto dataSection => dataSection.SectionIndex,
            SheetSectionDto sheetSection => sheetSection.SectionIndex,
            PageBreakSectionDto pageBreakSection => pageBreakSection.SectionIndex,
            _ => throw new ArgumentException("Unknown section type.", nameof(section))
        };
    }

    private sealed record SectionHandle(int SectionIndex, Action<int> SetSectionIndex);
}
