using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Features.CreateView;
using BlazorServer.Modules.Reports.Features.GetView;
using BlazorServer.Modules.Reports.Features.ListViews;
using BlazorServer.Modules.Reports.Features.UpdateView;
using BlazorServer.Modules.Reports.Persistence;
using BlazorServer.Modules.Reports.Public;
using DataColumnDefinition = BlazorServer.Modules.Reports.Domain.Views.DataSections.ColumnDefinition;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class ReportsViewManagementHandlersTargetModelTests
{
    [Test]
    public async Task CreateView_StoresNewOrganizationViewThroughPort()
    {
        var store = new RecordingViewStore();
        var handler = new CreateViewCommandHandler(store);
        var organizationId = new OrganizationId("org-1");

        var result = await handler.HandleAsync(new CreateViewCommand(organizationId, "Tender report"), CancellationToken.None);

        Assert.That(result, Is.TypeOf<ViewCreated>());
        var created = (ViewCreated)result;
        Assert.Multiple(() =>
        {
            Assert.That(created.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(created.Name, Is.EqualTo("Tender report"));
            Assert.That(store.Upserted, Is.SameAs(created.View));
            Assert.That(store.Upserted!.OrganizationId, Is.EqualTo(organizationId));
        });
    }

    [Test]
    public async Task ListViews_ReturnsViewsFromPortForOrganization()
    {
        var organizationId = new OrganizationId("org-1");
        var expected = ReconstitutedView(organizationId: organizationId);
        var store = new RecordingViewStore { AllViews = [expected] };
        var handler = new ListViewsQueryHandler(store);

        var result = await handler.HandleAsync(new ListViewsQuery(organizationId), CancellationToken.None);

        Assert.That(result, Is.TypeOf<ViewsListed>());
        Assert.That(((ViewsListed)result).Views, Is.EqualTo(new[] { expected }));
        Assert.That(store.ListedOrganizationId, Is.EqualTo(organizationId));
    }

    [Test]
    public async Task GetView_ReturnsViewFromPort()
    {
        var viewId = new ViewId("view-1");
        var expected = ReconstitutedView(viewId);
        var store = new RecordingViewStore { View = expected };
        var handler = new GetViewQueryHandler(store);

        var result = await handler.HandleAsync(new GetViewQuery(viewId), CancellationToken.None);

        Assert.That(result, Is.EqualTo(new ViewLoaded(expected)));
        Assert.That(store.LoadedViewId, Is.EqualTo(viewId));
    }

    [Test]
    public async Task UpdateView_StoresEditedViewThroughPort()
    {
        var organizationId = new OrganizationId("org-1");
        var viewId = new ViewId("view-1");
        var existing = ReconstitutedView(viewId, organizationId);
        var store = new RecordingViewStore { View = existing };
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(organizationId, viewId, "Updated tender report", "Updated description", []),
            CancellationToken.None);

        Assert.That(result, Is.TypeOf<ViewUpdated>());
        var updated = (ViewUpdated)result;
        Assert.Multiple(() =>
        {
            Assert.That(updated.ViewId, Is.EqualTo(viewId));
            Assert.That(updated.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(updated.Name, Is.EqualTo("Updated tender report"));
            Assert.That(updated.View, Is.SameAs(existing));
            Assert.That(store.Upserted, Is.SameAs(existing));
            Assert.That(store.Upserted!.Description, Is.EqualTo("Updated description"));
        });
    }

    [Test]
    public async Task UpdateView_AppliesEditedSectionsThroughViewStructureRules()
    {
        var organizationId = new OrganizationId("org-1");
        var viewId = new ViewId("view-1");
        var existing = ReconstitutedView(viewId, organizationId, sections: [DataSection("data-1"), SheetSection("sheet-1")]);
        var store = new RecordingViewStore { View = existing };
        var handler = new UpdateViewCommandHandler(store);

        var editedDataSection = DataSection("data-1");
        var addedSheetSection = SheetSection("sheet-2");
        var result = await handler.HandleAsync(
            new UpdateViewCommand(
                organizationId,
                viewId,
                "Updated tender report",
                "Updated description",
                [editedDataSection, addedSheetSection],
                ConfirmSectionDelete: true),
            CancellationToken.None);

        Assert.That(result, Is.TypeOf<ViewUpdated>());
        Assert.That(existing.Sections.Select(section => section.SectionId.Value), Is.EqualTo(new[] { "data-1", "sheet-2" }));
        Assert.That(store.Upserted, Is.SameAs(existing));
    }

    [Test]
    public async Task UpdateView_ReturnsValidationErrorForInvalidEditedSectionStructure()
    {
        var organizationId = new OrganizationId("org-1");
        var viewId = new ViewId("view-1");
        var existing = ReconstitutedView(viewId, organizationId, sections: [DataSection("section-1")]);
        var store = new RecordingViewStore { View = existing };
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(
                organizationId,
                viewId,
                "Updated tender report",
                "Updated description",
                [SheetSection("section-1")],
                ConfirmSectionDelete: true),
            CancellationToken.None);

        Assert.That(result, Is.TypeOf<InvalidUpdateViewRequest>());
        var invalidRequest = (InvalidUpdateViewRequest)result;
        Assert.That(invalidRequest.ValidationErrors.Select(error => error.Code), Does.Contain("view.section_type.changed"));
        Assert.That(existing.Name, Is.EqualTo("Tender report"));
        Assert.That(store.Upserted, Is.Null);
    }

    [Test]
    public async Task UpdateView_RequiresExplicitConfirmationBeforeDeletingSection()
    {
        var organizationId = new OrganizationId("org-1");
        var viewId = new ViewId("view-1");
        var existing = ReconstitutedView(viewId, organizationId, sections: [DataSection("data-1")]);
        var store = new RecordingViewStore { View = existing };
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(organizationId, viewId, "Updated tender report", "Updated description", [], ConfirmSectionDelete: false),
            CancellationToken.None);

        Assert.That(result, Is.TypeOf<InvalidUpdateViewRequest>());
        var invalidRequest = (InvalidUpdateViewRequest)result;
        Assert.That(invalidRequest.ValidationErrors.Select(error => error.Code), Does.Contain("view.section_delete.confirmation_required"));
        Assert.That(existing.Sections.Select(section => section.SectionId.Value), Is.EqualTo(new[] { "data-1" }));
        Assert.That(store.Upserted, Is.Null);
    }

    [Test]
    public async Task UpdateView_ReturnsValidationErrorForArchivedView()
    {
        var organizationId = new OrganizationId("org-1");
        var viewId = new ViewId("view-1");
        var existing = ReconstitutedView(viewId, organizationId, isArchived: true);
        var store = new RecordingViewStore { View = existing };
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(organizationId, viewId, "Updated tender report", "Updated description", []),
            CancellationToken.None);

        Assert.That(result, Is.TypeOf<InvalidUpdateViewRequest>());
        var invalidRequest = (InvalidUpdateViewRequest)result;
        Assert.That(invalidRequest.ValidationErrors.Select(error => error.Code), Does.Contain("view.archived"));
        Assert.That(existing.Name, Is.EqualTo("Tender report"));
        Assert.That(store.Upserted, Is.Null);
    }

    [Test]
    public async Task UpdateView_DoesNotCreateMissingView()
    {
        var viewId = new ViewId("missing-view");
        var store = new RecordingViewStore();
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(new OrganizationId("org-1"), viewId, "Updated", "", []),
            CancellationToken.None);

        Assert.That(result, Is.EqualTo(new ViewToUpdateNotFound(viewId)));
        Assert.That(store.Upserted, Is.Null);
    }

    [Test]
    public async Task UpdateView_DoesNotUpdateViewFromAnotherOrganization()
    {
        var viewId = new ViewId("view-1");
        var requestedOrganizationId = new OrganizationId("org-2");
        var store = new RecordingViewStore { View = ReconstitutedView(viewId, new OrganizationId("org-1")) };
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(requestedOrganizationId, viewId, "Updated", "", []),
            CancellationToken.None);

        Assert.That(result, Is.EqualTo(new ViewUpdateOrganizationMismatch(viewId, requestedOrganizationId)));
        Assert.That(store.Upserted, Is.Null);
    }

    private sealed class RecordingViewStore : IViewStore
    {
        public List<View> AllViews { get; init; } = [];
        public View? View { get; init; }
        public View? Upserted { get; private set; }
        public OrganizationId? ListedOrganizationId { get; private set; }
        public ViewId? LoadedViewId { get; private set; }

        public Task DeleteAsync(ViewId viewId) => Task.CompletedTask;

        public Task<List<View>> GetAllAsync(OrganizationId orgId)
        {
            ListedOrganizationId = orgId;
            return Task.FromResult(AllViews);
        }

        public Task<View?> GetAsync(ViewId viewId)
        {
            LoadedViewId = viewId;
            return Task.FromResult(View);
        }

        public Task UpsertAsync(View view)
        {
            Upserted = view;
            return Task.CompletedTask;
        }
    }

    private static View ReconstitutedView(
        ViewId? viewId = null,
        OrganizationId? organizationId = null,
        string name = "Tender report",
        string description = "",
        bool isArchived = false,
        IReadOnlyList<IViewSection>? sections = null)
        => View.Reconstitute(
            viewId ?? new ViewId("view-1"),
            organizationId ?? new OrganizationId("org-1"),
            name,
            description,
            isArchived,
            sections ?? []);

    private static DataSection DataSection(string sectionId)
        => BlazorServer.Modules.Reports.Domain.Views.DataSections.DataSection.Reconstitute(
            new SectionId(sectionId),
            [new RowDefinition(8)],
            [new DataColumnDefinition(1)]);

    private static SheetSection SheetSection(string sectionId)
        => BlazorServer.Modules.Reports.Domain.Views.SheetSections.SheetSection.Reconstitute(new SectionId(sectionId));
}
