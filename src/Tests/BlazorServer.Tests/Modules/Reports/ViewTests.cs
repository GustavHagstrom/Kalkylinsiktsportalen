using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Public;
using DataColumnDefinition = BlazorServer.Modules.Reports.Domain.Views.DataSections.ColumnDefinition;
using SheetSectionModel = BlazorServer.Modules.Reports.Domain.Views.SheetSections.SheetSection;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ViewTargetModelTests
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

        Assert.That(Assert.Throws<ReportsDomainException>(() => view.Rename("Blocked"))!.Code, Is.EqualTo("view.archived"));
        Assert.That(Assert.Throws<ReportsDomainException>(() => view.ChangePageFormat(PageFormat.A5))!.Code, Is.EqualTo("view.archived"));
    }

    [Test]
    public void AddSections_AppendsTypedSectionsWithStableIdentities()
    {
        var view = ReconstitutedView(sections: [SheetSection("sheet-1")]);

        var section = view.AddDataSection();

        Assert.Multiple(() =>
        {
            Assert.That(section.SectionId.Value, Is.Not.Empty);
            Assert.That(view.GetIndexOfSection(section), Is.EqualTo(1));
            Assert.That(section.Rows, Has.Count.EqualTo(3));
            Assert.That(section.Columns, Has.Count.EqualTo(3));
            Assert.That(section.Cells, Is.Empty);
        });
    }

    [Test]
    public void RemoveSection_RemovesOnlyTheTargetSectionWithoutChangingRemainingIdentity()
    {
        var removedSection = DataSection("removed-section");
        var remainingSection = SheetSection("remaining-section");
        var view = ReconstitutedView(sections: [removedSection, remainingSection]);

        view.RemoveSection(removedSection.SectionId);

        Assert.Multiple(() =>
        {
            Assert.That(view.Sections, Is.EqualTo(new[] { remainingSection }));
            Assert.That(view.Sections.Single().SectionId.Value, Is.EqualTo("remaining-section"));
        });
    }

    [Test]
    public void MoveSection_ReordersSectionsWithoutChangingSectionIdentity()
    {
        var first = DataSection("data-1");
        var second = SheetSection("sheet-1");
        var view = ReconstitutedView(sections: [first, second]);

        view.MoveSection(first.SectionId, 1);

        Assert.That(view.Sections.Select(section => section.SectionId.Value), Is.EqualTo(new[] { "sheet-1", "data-1" }));
    }

    [Test]
    public void DuplicateCopiesDefinitionWithNewViewAndSectionIdentities()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report", "For review");
        var dataSection = view.AddDataSection();
        dataSection.EditCellExpression(0, 0, "Heading");
        var sheetSection = view.AddSheetSection(SheetKind.SummarySheet);
        var duplicate = view.Duplicate("Tender report copy");
        var duplicatedSheetSection = duplicate.Sections
            .OfType<SheetSectionModel>()
            .Single();

        Assert.Multiple(() =>
        {
            Assert.That(duplicate.Id, Is.Not.EqualTo(view.Id));
            Assert.That(duplicate.OrganizationId, Is.EqualTo(view.OrganizationId));
            Assert.That(duplicate.Name, Is.EqualTo("Tender report copy"));
            Assert.That(duplicate.Description, Is.EqualTo(view.Description));
            Assert.That(duplicate.IsArchived, Is.False);
            Assert.That(duplicate.Sections.Select(section => section.SectionId), Is.Not.EquivalentTo(view.Sections.Select(section => section.SectionId)));
            Assert.That(duplicate.Sections.Select(section => section.GetType()), Is.EqualTo(view.Sections.Select(section => section.GetType())));
            Assert.That(duplicatedSheetSection.SheetKind, Is.EqualTo(sheetSection.SheetKind));
        });
    }

    [Test]
    public void SheetSectionsCarrySheetKindWhenCreatedAndReconstituted()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        var created = view.AddSheetSection(SheetKind.SummarySheet);
        var reconstituted = SheetSectionModel.Reconstitute(
            new SectionId("sheet-1"),
            SheetSectionModel.DefaultColumns(),
            SheetKind.SummarySheet);

        Assert.Multiple(() =>
        {
            Assert.That(created.SheetKind, Is.EqualTo(SheetKind.SummarySheet));
            Assert.That(reconstituted.SheetKind, Is.EqualTo(SheetKind.SummarySheet));
        });
    }

    [Test]
    public void ReplaceSections_ReplacesStructureAndPreservesGivenOrder()
    {
        var view = ReconstitutedView(sections: [DataSection("data-1"), SheetSection("sheet-1")]);
        var editedDataSection = DataSection("data-1");
        var addedSheetSection = SheetSection("sheet-2");

        view.ReplaceSections([addedSheetSection, editedDataSection]);

        Assert.That(view.Sections.Select(section => section.SectionId.Value), Is.EqualTo(new[] { "sheet-2", "data-1" }));
    }

    [Test]
    public void ReplaceSections_RejectsDuplicateSectionId()
    {
        var view = View.Create(new OrganizationId("org-1"), "Tender report");

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ReplaceSections([DataSection("section-1"), SheetSection("section-1")]));

        Assert.That(exception!.Code, Is.EqualTo("view.section_id.duplicate"));
    }

    [Test]
    public void ReplaceSections_RejectsTypeChangeForExistingSectionId()
    {
        var view = ReconstitutedView(sections: [DataSection("section-1")]);

        var exception = Assert.Throws<ReportsDomainException>(() =>
            view.ReplaceSections([SheetSection("section-1")]));

        Assert.That(exception!.Code, Is.EqualTo("view.section_type.changed"));
    }

    private static View ReconstitutedView(IReadOnlyList<IViewSection>? sections = null)
        => View.Reconstitute(
            new ViewId("view-1"),
            new OrganizationId("org-1"),
            "Tender report",
            "",
            isArchived: false,
            sections ?? []);

    private static DataSection DataSection(string sectionId)
        => BlazorServer.Modules.Reports.Domain.Views.DataSections.DataSection.Reconstitute(
            new SectionId(sectionId),
            [new RowDefinition(8)],
            [new DataColumnDefinition(1)]);

    private static SheetSection SheetSection(string sectionId)
        => SheetSectionModel.Reconstitute(
            new SectionId(sectionId),
            SheetSectionModel.DefaultColumns(),
            SheetKind.TenderSheet);
}
