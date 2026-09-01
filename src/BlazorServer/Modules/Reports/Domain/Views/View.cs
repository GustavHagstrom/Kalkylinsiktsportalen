using BlazorServer.Identity;
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
    public static View Reconstitute(ViewId id, OrganizationId organizationId, string name, string description, bool isArchived, IReadOnlyList<IViewSection> sections, PageFormat? pageFormat = null)
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
        view.ReplaceSectionsForHydration(sections);
        return view;
    }
    public View Duplicate(string name)
    {
        var duplicate = Create(OrganizationId, name, Description);
        duplicate.ChangePageFormat(PageFormat);

        foreach (var section in _sections)
        {
            duplicate.AddSection(section.Clone());
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
        var section = DataSection.CreateDefault();
        AddSection(section);
        return section;
    }
    public SheetSection AddSheetSection(SheetKind sheetKind)
    {
        var section = SheetSection.Create(sheetKind);
        AddSection(section);
        return section;
    }
    private void AddSection(IViewSection section)
    {
        EnsureActive();
        
        if (_sections.Any(existing => existing.SectionId == section.SectionId))
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

        var index = _sections.FindIndex(section => section.SectionId == id);
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

        var currentIndex = _sections.FindIndex(section => section.SectionId == id);
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
    public void ReplaceSections(IReadOnlyList<IViewSection> sections)
    {
        EnsureActive();
        ValidateSections(sections, _sections);

        _sections.Clear();
        _sections.AddRange(sections);
    }
    public int GetIndexOfSection(IViewSection section)
    {
        return _sections.IndexOf(section);
    }

    private void ReplaceSectionsForHydration(IReadOnlyList<IViewSection> sections)
    {
        ValidateSections(sections, []);
        _sections.AddRange(sections);
    }

    private static void ValidateSections(
        IReadOnlyList<IViewSection> sections,
        IReadOnlyList<IViewSection> existingSections)
    {
        var sectionIds = new HashSet<SectionId>();
        foreach (var section in sections)
        {
            if (!sectionIds.Add(section.SectionId))
            {
                throw new ReportsDomainException(
                    "view.section_id.duplicate",
                    "A view cannot contain duplicate section identities.");
            }

            var existingSection = existingSections.FirstOrDefault(existing => existing.SectionId == section.SectionId);
            if (existingSection is not null && existingSection.GetType() != section.GetType())
            {
                throw new ReportsDomainException(
                    "view.section_type.changed",
                    "An existing section cannot change type.");
            }
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

}
