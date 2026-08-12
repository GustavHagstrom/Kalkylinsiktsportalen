using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ViewTests
{
    [Test]
    public void Create_BuildsActiveOrganizationViewWithMetadataDefaults()
    {
        var organizationId = new OrganizationId("org-1");

        var view = View.Create(organizationId, "Tender report", "For review");

        Assert.Multiple(() =>
        {
            Assert.That(view.Id.IsEmpty, Is.False);
            Assert.That(view.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(view.Name, Is.EqualTo("Tender report"));
            Assert.That(view.Description, Is.EqualTo("For review"));
            Assert.That(view.PageFormat, Is.EqualTo(PageFormat.A4));
            Assert.That(view.IsArchived, Is.False);
            Assert.That(view.Sections, Is.Empty);
        });
    }

    [Test]
    public void MetadataAndLifecycleChangesAreExplicitAndBlockedWhenArchived()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        view.Rename("Updated report");
        view.UpdateDescription("Updated description");
        view.ChangePageFormat(PageFormat.A3);
        view.Archive();

        Assert.Multiple(() =>
        {
            Assert.That(view.Name, Is.EqualTo("Updated report"));
            Assert.That(view.Description, Is.EqualTo("Updated description"));
            Assert.That(view.PageFormat, Is.EqualTo(PageFormat.A3));
            Assert.That(view.IsArchived, Is.True);
        });

        var exception = Assert.Throws<ReportsDomainException>(() => view.Rename("Blocked"));
        Assert.That(exception!.Code, Is.EqualTo("view.archived"));
        Assert.That(Assert.Throws<ReportsDomainException>(() => view.ChangePageFormat(PageFormat.A5))!.Code, Is.EqualTo("view.archived"));

        view.Reactivate();
        view.Rename("Reactivated report");

        Assert.Multiple(() =>
        {
            Assert.That(view.IsArchived, Is.False);
            Assert.That(view.Name, Is.EqualTo("Reactivated report"));
        });
    }

    [Test]
    public void SheetSectionOwnsChangeEnablementWithLegacyViewDefaultDuringReconstitution()
    {
        var newView = View.Create(new OrganizationId("org-1"), "Tender report");
        var newSheetSection = newView.AddSheetSection();

        var legacyEnabledView = View.Reconstitute(
            new ViewId("view-1"),
            new OrganizationId("org-1"),
            "Tender report",
            "",
            enableChanges: true,
            isArchived: false,
            [],
            [new SheetSection { SectionId = "sheet-1", SectionIndex = 0 }]);

        Assert.Multiple(() =>
        {
            Assert.That(newSheetSection.EnableChanges, Is.False);
            Assert.That(legacyEnabledView.LegacySheetSections.Single().EnableChanges, Is.True);
        });
    }

    [Test]
    public void AddMoveAndRemoveSectionsUseOrderedAggregateCollection()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        var dataSection = view.AddDataSection();
        var sheetSection = view.AddSheetSection();

        view.MoveSection(sheetSection.SectionId, 0);

        Assert.Multiple(() =>
        {
            Assert.That(view.Sections.Select(section => section.SectionId), Is.EqualTo(new[]
            {
                sheetSection.SectionId,
                dataSection.SectionId
            }));
            Assert.That(sheetSection.SectionIndex, Is.EqualTo(0));
            Assert.That(dataSection.SectionIndex, Is.EqualTo(1));
        });

        view.RemoveSection(sheetSection.SectionId);

        Assert.Multiple(() =>
        {
            Assert.That(view.Sections.Select(section => section.SectionId), Is.EqualTo(new[] { dataSection.SectionId }));
            Assert.That(dataSection.SectionIndex, Is.EqualTo(0));
            Assert.That(view.LegacySheetSections, Is.Empty);
        });
    }

    [Test]
    public void DuplicateCopiesDefinitionWithNewViewAndSectionIdentities()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report", "For review");
        var dataSection = view.AddDataSection();
        dataSection.GetCell(0, 0).RawValue = "Heading";
        var sheetSection = view.AddSheetSection();
        sheetSection.Content = "TenderSheet";
        sheetSection.SetEnableChanges(true);

        var duplicate = view.Duplicate("Tender report copy");

        Assert.Multiple(() =>
        {
            Assert.That(duplicate.Id, Is.Not.EqualTo(view.Id));
            Assert.That(duplicate.OrganizationId, Is.EqualTo(view.OrganizationId));
            Assert.That(duplicate.Name, Is.EqualTo("Tender report copy"));
            Assert.That(duplicate.Description, Is.EqualTo(view.Description));
            Assert.That(duplicate.IsArchived, Is.False);
            Assert.That(duplicate.Sections.Select(section => section.SectionId), Is.Not.EquivalentTo(view.Sections.Select(section => section.SectionId)));
            Assert.That(duplicate.LegacyDataSections[0].DataCells["R0C0"].RawValue, Is.EqualTo("Heading"));
            Assert.That(duplicate.LegacySheetSections[0].Content, Is.EqualTo("TenderSheet"));
            Assert.That(duplicate.LegacySheetSections[0].EnableChanges, Is.True);
        });
    }

    [Test]
    public void AddDataSection_AddsDefaultEditableGridAtNextSectionIndex()
    {
        var view = ReconstitutedView(sheetSections: [new SheetSection { SectionIndex = 0 }]);
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
    public void RemoveDataSection_CollapsesFollowingSectionIndexesWithoutChangingSectionIdentity()
    {
        var removedSection = new DataSection { SectionId = "removed-section", SectionIndex = 0 };
        var remainingSection = new SheetSection { SectionId = "remaining-section", SectionIndex = 1 };
        var view = ReconstitutedView([removedSection], [remainingSection]);
        view.RemoveDataSection(removedSection);

        Assert.Multiple(() =>
        {
            Assert.That(view.LegacyDataSections, Is.Empty);
            Assert.That(remainingSection.SectionId, Is.EqualTo("remaining-section"));
            Assert.That(remainingSection.SectionIndex, Is.EqualTo(0));
        });
    }

    [Test]
    public void Reconstitute_AssignsMissingSectionIdsForHydratedLegacySections()
    {
        var view = ReconstitutedView(
            [new DataSection { SectionId = "", SectionIndex = 0 }],
            [new SheetSection { SectionId = "", SectionIndex = 1 }]);

        Assert.Multiple(() =>
        {
            Assert.That(view.LegacyDataSections[0].SectionId, Is.Not.Empty);
            Assert.That(view.LegacySheetSections[0].SectionId, Is.Not.Empty);
            Assert.That(view.LegacyDataSections[0].SectionId, Is.Not.EqualTo(view.LegacySheetSections[0].SectionId));
        });
    }

    [Test]
    public void ApplyLegacySectionBridge_ReplacesStructureAndNormalizesOrderInsideView()
    {
        var view = ReconstitutedView(
            [new DataSection { SectionId = "data-1", SectionIndex = 0 }],
            [new SheetSection { SectionId = "sheet-1", SectionIndex = 1 }]);

        var editedDataSection = new DataSection { SectionId = "data-1", SectionIndex = 1 };
        var addedSheetSection = new SheetSection { SectionId = "sheet-2", SectionIndex = 0 };

        view.ApplyLegacySectionBridge([editedDataSection], [addedSheetSection]);

        Assert.Multiple(() =>
        {
            Assert.That(view.LegacyDataSections.Select(section => section.SectionId), Is.EqualTo(new[] { editedDataSection.SectionId }));
            Assert.That(view.LegacySheetSections.Select(section => section.SectionId), Is.EqualTo(new[] { addedSheetSection.SectionId }));
            Assert.That(addedSheetSection.SectionIndex, Is.EqualTo(0));
            Assert.That(editedDataSection.SectionIndex, Is.EqualTo(1));
        });
    }

    [Test]
    public void ApplyLegacySectionBridge_RejectsBlankSectionId()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyLegacySectionBridge([new DataSection { SectionId = "", SectionIndex = 0 }], []));

        Assert.That(exception!.Code, Is.EqualTo("view.section_id.required"));
    }

    [Test]
    public void ApplyLegacySectionBridge_RejectsDuplicateSectionId()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyLegacySectionBridge(
                [new DataSection { SectionId = "section-1", SectionIndex = 0 }],
                [new SheetSection { SectionId = "section-1", SectionIndex = 1 }]));

        Assert.That(exception!.Code, Is.EqualTo("view.section_id.duplicate"));
    }

    [Test]
    public void ApplyLegacySectionBridge_RejectsTypeChangeForExistingSectionId()
    {
        var view = ReconstitutedView([new DataSection { SectionId = "section-1", SectionIndex = 0 }]);

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyLegacySectionBridge([], [new SheetSection { SectionId = "section-1", SectionIndex = 0 }]));

        Assert.That(exception!.Code, Is.EqualTo("view.section_type.changed"));
    }

    [Test]
    public void ApplyLegacySectionBridge_RejectsNonContiguousSectionIndexes()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ApplyLegacySectionBridge([new DataSection { SectionId = "section-1", SectionIndex = 1 }], []));

        Assert.That(exception!.Code, Is.EqualTo("view.section_order.invalid"));
    }

    [Test]
    public void PublicSectionListsDoNotExposeMutableAggregateChildren()
    {
        var dataSection = new DataSection { SectionId = "data-1", SectionIndex = 0 };
        var sheetSection = new SheetSection { SectionId = "sheet-1", SectionIndex = 1, Content = "TenderSheet" };
        var view = ReconstitutedView([dataSection], [sheetSection]);

        view.LegacyDataSections[0].SectionId = "changed-data";
        view.LegacySheetSections[0].SectionId = "changed-sheet";
        view.LegacySheetSections[0].Content = "ChangedSheet";

        Assert.Multiple(() =>
        {
            Assert.That(view.Sections.Select(section => section.SectionId), Is.EqualTo(new[] { "data-1", "sheet-1" }));
            Assert.That(view.LegacyDataSections[0].SectionId, Is.EqualTo("data-1"));
            Assert.That(view.LegacySheetSections[0].SectionId, Is.EqualTo("sheet-1"));
            Assert.That(view.LegacySheetSections[0].Content, Is.EqualTo("TenderSheet"));
        });
    }

    private static View ReconstitutedView(
        IReadOnlyList<DataSection>? dataSections = null,
        IReadOnlyList<SheetSection>? sheetSections = null)
        => View.Reconstitute(
            new ViewId("view-1"),
            new OrganizationId("org-1"),
            "Tender report",
            "",
            enableChanges: false,
            isArchived: false,
            dataSections ?? [],
            sheetSections ?? []);
}
