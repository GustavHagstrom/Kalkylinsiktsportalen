using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using BlazorServer.Modules.Reports.Domain;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ViewTests
{
    [Test]
    public void Create_BuildsOrganizationOwnedActiveViewWithChangeEditsDisabled()
    {
        var organizationId = new OrganizationId("org-1");

        var view = View.Create(organizationId, " Tender report ", " For review ");

        Assert.Multiple(() =>
        {
            Assert.That(view.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(view.Name, Is.EqualTo("Tender report"));
            Assert.That(view.Description, Is.EqualTo("For review"));
            Assert.That(view.EnableChanges, Is.False);
            Assert.That(view.IsArchived, Is.False);
            Assert.That(view.SectionCount, Is.EqualTo(0));
        });
    }

    [Test]
    public void UpdateMetadata_RenamesAndUpdatesDescriptionWithoutChangingOrganization()
    {
        var organizationId = new OrganizationId("org-1");
        var view = View.Create(organizationId, "Tender report");

        view.UpdateMetadata(" Updated tender report ", " Updated description ");

        Assert.Multiple(() =>
        {
            Assert.That(view.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(view.Name, Is.EqualTo("Updated tender report"));
            Assert.That(view.Description, Is.EqualTo("Updated description"));
        });
    }

    [Test]
    public void Archive_BlocksNormalEditOperationsUntilReactivated()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");
        view.AddDataSection();

        view.Archive();

        var exception = Assert.Throws<ReportsDomainException>(() => view.UpdateMetadata("Updated", ""));
        Assert.Multiple(() =>
        {
            Assert.That(view.IsArchived, Is.True);
            Assert.That(exception!.Code, Is.EqualTo("view.archived"));
            Assert.That(Assert.Throws<ReportsDomainException>(() => view.AddDataSection())!.Code, Is.EqualTo("view.archived"));
            Assert.That(Assert.Throws<ReportsDomainException>(() => view.RemoveSection(view.DataSections[0].SectionId))!.Code, Is.EqualTo("view.archived"));
        });

        view.Reactivate();
        view.UpdateMetadata("Updated", "");

        Assert.That(view.Name, Is.EqualTo("Updated"));
    }

    [Test]
    public void AddDataSection_AddsDefaultEditableGridAtNextSectionIndex()
    {
        var view = new View
        {
            SheetSections = new List<SheetSection>
            {
                new() { SectionIndex = 0 }
            }
        };
        var section = view.AddDataSection();

        Assert.Multiple(() =>
        {
            Assert.That(section.SectionId, Is.Not.Empty);
            Assert.That(section.SectionIndex, Is.EqualTo(1));
            Assert.That(section.RowsCount, Is.EqualTo(3));
            Assert.That(section.ColumnsCount, Is.EqualTo(3));
            Assert.That(section.DataCells.Keys, Is.EquivalentTo(new[]
            {
                "R0C0", "R0C1", "R0C2",
                "R1C0", "R1C1", "R1C2",
                "R2C0", "R2C1", "R2C2"
            }));
        });
    }

    [Test]
    public void AddSectionMoveSectionAndRemoveSection_KeepOrderAsCollectionPosition()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        var first = view.AddSection(ViewSectionType.Data);
        var second = view.AddSection(ViewSectionType.Sheet);
        var third = view.AddSection(ViewSectionType.Data);

        view.MoveSection(third.SectionId, 0);
        view.RemoveSection(second.SectionId);

        Assert.Multiple(() =>
        {
            Assert.That(view.Sections.Select(section => section.SectionId), Is.EqualTo(new[] { third.SectionId, first.SectionId }));
            Assert.That(view.DataSections.Select(section => section.SectionIndex), Is.EqualTo(new[] { 1, 0 }));
            Assert.That(view.SheetSections, Is.Empty);
        });
    }

    [Test]
    public void RemoveDataSection_CollapsesFollowingSectionIndexesWithoutChangingSectionIdentity()
    {
        var removedSection = new DataSection { SectionId = "removed-section", SectionIndex = 0 };
        var remainingSection = new SheetSection { SectionId = "remaining-section", SectionIndex = 1 };
        var view = new View
        {
            DataSections = new List<DataSection> { removedSection },
            SheetSections = new List<SheetSection> { remainingSection }
        };
        view.RemoveDataSection(removedSection);

        Assert.Multiple(() =>
        {
            Assert.That(view.DataSections, Is.Empty);
            Assert.That(remainingSection.SectionId, Is.EqualTo("remaining-section"));
            Assert.That(remainingSection.SectionIndex, Is.EqualTo(0));
        });
    }

    [Test]
    public void Normalize_AssignsMissingSectionIdsForHydratedLegacySections()
    {
        var view = new View
        {
            DataSections = new List<DataSection> { new() { SectionId = "", SectionIndex = 0 } },
            SheetSections = new List<SheetSection> { new() { SectionId = "", SectionIndex = 1 } }
        };

        view.Normalize();

        Assert.Multiple(() =>
        {
            Assert.That(view.DataSections[0].SectionId, Is.Not.Empty);
            Assert.That(view.SheetSections[0].SectionId, Is.Not.Empty);
            Assert.That(view.DataSections[0].SectionId, Is.Not.EqualTo(view.SheetSections[0].SectionId));
        });
    }

    [Test]
    public void ApplyEditedSections_ReplacesStructureAndNormalizesOrderInsideView()
    {
        var view = new View
        {
            DataSections = new List<DataSection>
            {
                new() { SectionId = "data-1", SectionIndex = 0 }
            },
            SheetSections = new List<SheetSection>
            {
                new() { SectionId = "sheet-1", SectionIndex = 1 }
            }
        };

        var editedDataSection = new DataSection { SectionId = "data-1", SectionIndex = 1 };
        var addedSheetSection = new SheetSection { SectionId = "sheet-2", SectionIndex = 0 };

        view.ApplyEditedSections([editedDataSection], [addedSheetSection]);

        Assert.Multiple(() =>
        {
            Assert.That(view.DataSections, Is.EqualTo(new[] { editedDataSection }));
            Assert.That(view.SheetSections, Is.EqualTo(new[] { addedSheetSection }));
            Assert.That(addedSheetSection.SectionIndex, Is.EqualTo(0));
            Assert.That(editedDataSection.SectionIndex, Is.EqualTo(1));
        });
    }

    [Test]
    public void ApplyEditedSections_RejectsBlankSectionId()
    {
        var view = new View();

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyEditedSections([new DataSection { SectionId = "", SectionIndex = 0 }], []));

        Assert.That(exception!.Code, Is.EqualTo("view.section_id.required"));
    }

    [Test]
    public void ApplyEditedSections_RejectsDuplicateSectionId()
    {
        var view = new View();

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyEditedSections(
                [new DataSection { SectionId = "section-1", SectionIndex = 0 }],
                [new SheetSection { SectionId = "section-1", SectionIndex = 1 }]));

        Assert.That(exception!.Code, Is.EqualTo("view.section_id.duplicate"));
    }

    [Test]
    public void ApplyEditedSections_RejectsTypeChangeForExistingSectionId()
    {
        var view = new View
        {
            DataSections = new List<DataSection>
            {
                new() { SectionId = "section-1", SectionIndex = 0 }
            }
        };

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyEditedSections([], [new SheetSection { SectionId = "section-1", SectionIndex = 0 }]));

        Assert.That(exception!.Code, Is.EqualTo("view.section_type.changed"));
    }

    [Test]
    public void ApplyEditedSections_RejectsNonContiguousSectionIndexes()
    {
        var view = new View();

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyEditedSections([new DataSection { SectionId = "section-1", SectionIndex = 1 }], []));

        Assert.That(exception!.Code, Is.EqualTo("view.section_order.invalid"));
    }

    [Test]
    public void Duplicate_CopiesDefinitionAndSettingsButCreatesNewSectionIdentities()
    {
        var source = View.Create(new OrganizationId("org-1"), "Tender report", "For review");
        source.EnableChangeEdits();
        var sourceDataSection = source.AddDataSection();
        sourceDataSection.GetCell(0, 0).RawValue = "Hello";
        var sourceSheetSection = source.AddSheetSection();
        sourceSheetSection.Content = "TenderSheet";

        var copy = source.Duplicate("Copied report");

        Assert.Multiple(() =>
        {
            Assert.That(copy.Id, Is.Not.EqualTo(source.Id));
            Assert.That(copy.OrganizationId, Is.EqualTo(source.OrganizationId));
            Assert.That(copy.Name, Is.EqualTo("Copied report"));
            Assert.That(copy.Description, Is.EqualTo(source.Description));
            Assert.That(copy.EnableChanges, Is.True);
            Assert.That(copy.IsArchived, Is.False);
            Assert.That(copy.Sections.Select(section => section.SectionType), Is.EqualTo(source.Sections.Select(section => section.SectionType)));
            Assert.That(copy.Sections.Select(section => section.SectionId), Is.Not.EquivalentTo(source.Sections.Select(section => section.SectionId)));
            Assert.That(copy.DataSections[0].DataCells["R0C0"].RawValue, Is.EqualTo("Hello"));
            Assert.That(copy.SheetSections[0].Content, Is.EqualTo("TenderSheet"));
        });
    }
}
