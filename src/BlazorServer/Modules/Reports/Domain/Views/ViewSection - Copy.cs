using DataSectionModel = BlazorServer.Modules.Reports.Domain.Views.DataSection.DataSection;
using SheetSectionModel = BlazorServer.Modules.Reports.Domain.Views.SheetSection.SheetSection;

namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed class ViewSectionLegacy
{
    private string _sectionId = string.Empty;
    private int _sectionIndex;

    private ViewSectionLegacy(DataSectionModel? dataSection, SheetSectionModel? sheetSection)
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

    public static ViewSectionLegacy FromData(DataSectionModel section)
        => new(section, null);

    public static ViewSectionLegacy FromSheet(SheetSectionModel section)
        => new(null, section);

    public void EnsureSectionId()
    {
        if (SectionType == ViewSectionType.Data)
        {
            if (string.IsNullOrWhiteSpace(DataSection!.SectionId))
                DataSection.SectionId = Views.SectionId.New();

            if (string.IsNullOrWhiteSpace(_sectionId))
                _sectionId = DataSection.SectionId;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(SheetSection!.SectionId))
                SheetSection.SectionId = Views.SectionId.New();

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
