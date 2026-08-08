using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using BlazorServer.Modules.Reports.Domain;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ViewTests
{
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
}
