using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Features.CreateView;
using BlazorServer.Modules.Reports.Features.GetView;
using BlazorServer.Modules.Reports.Features.ListViews;
using BlazorServer.Modules.Reports.Features.UpdateView;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Persistence;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class ReportsViewManagementHandlersTests
{
    [Test]
    public async Task CreateView_StoresNewOrganizationViewThroughPort()
    {
        var store = new RecordingViewStore();
        var handler = new CreateViewCommandHandler(store);
        var organizationId = new OrganizationId("org-1");

        var result = await handler.HandleAsync(
            new CreateViewCommand(organizationId, "Tender report"),
            CancellationToken.None);

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
        var expected = new ViewDto { OrganizationId = organizationId, Name = "Tender report" };
        var store = new RecordingViewStore { AllViews = [expected] };
        var handler = new ListViewsQueryHandler(store);

        var result = await handler.HandleAsync(new ListViewsQuery(organizationId), CancellationToken.None);

        Assert.That(result, Is.TypeOf<ViewsListed>());
        var listed = (ViewsListed)result;
        Assert.Multiple(() =>
        {
            Assert.That(listed.Views, Is.EqualTo(new[] { expected }));
            Assert.That(store.ListedOrganizationId, Is.EqualTo(organizationId));
        });
    }

    [Test]
    public async Task GetView_ReturnsViewFromPort()
    {
        var viewId = new ViewId("view-1");
        var expected = new ViewDto { Id = viewId, Name = "Tender report" };
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
        var existing = new ViewDto
        {
            Id = viewId,
            OrganizationId = organizationId,
            Name = "Tender report"
        };
        var store = new RecordingViewStore { View = existing };
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(
                organizationId,
                viewId,
                "Updated tender report",
                "Updated description",
                [],
                []),
            CancellationToken.None);

        Assert.That(result, Is.TypeOf<ViewUpdated>());
        var updated = (ViewUpdated)result;
        Assert.Multiple(() =>
        {
            Assert.That(updated.ViewId, Is.EqualTo(viewId));
            Assert.That(updated.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(updated.Name, Is.EqualTo("Updated tender report"));
            Assert.That(updated.View, Is.SameAs(existing));
            Assert.That(store.LoadedViewId, Is.EqualTo(viewId));
            Assert.That(store.Upserted, Is.SameAs(existing));
            Assert.That(store.Upserted!.Description, Is.EqualTo("Updated description"));
        });
    }

    [Test]
    public async Task UpdateView_DoesNotCreateMissingView()
    {
        var viewId = new ViewId("missing-view");
        var store = new RecordingViewStore();
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(new OrganizationId("org-1"), viewId, "Updated", "", [], []),
            CancellationToken.None);

        Assert.That(result, Is.EqualTo(new ViewToUpdateNotFound(viewId)));
        Assert.That(store.Upserted, Is.Null);
    }

    [Test]
    public async Task UpdateView_DoesNotUpdateViewFromAnotherOrganization()
    {
        var viewId = new ViewId("view-1");
        var requestedOrganizationId = new OrganizationId("org-2");
        var store = new RecordingViewStore
        {
            View = new ViewDto
            {
                Id = viewId,
                OrganizationId = new OrganizationId("org-1"),
                Name = "Tender report"
            }
        };
        var handler = new UpdateViewCommandHandler(store);

        var result = await handler.HandleAsync(
            new UpdateViewCommand(requestedOrganizationId, viewId, "Updated", "", [], []),
            CancellationToken.None);

        Assert.That(result, Is.EqualTo(new ViewUpdateOrganizationMismatch(viewId, requestedOrganizationId)));
        Assert.That(store.Upserted, Is.Null);
    }

    private sealed class RecordingViewStore : IViewStore
    {
        public List<ViewDto> AllViews { get; init; } = [];
        public ViewDto? View { get; init; }
        public ViewDto? Upserted { get; private set; }
        public OrganizationId? ListedOrganizationId { get; private set; }
        public ViewId? LoadedViewId { get; private set; }

        public Task DeleteAsync(ViewId viewId) => Task.CompletedTask;

        public Task<List<ViewDto>> GetAllAsync(OrganizationId orgId)
        {
            ListedOrganizationId = orgId;
            return Task.FromResult(AllViews);
        }

        public Task<ViewDto?> GetAsync(ViewId viewId)
        {
            LoadedViewId = viewId;
            return Task.FromResult(View);
        }

        public Task UpsertAsync(ViewDto view)
        {
            Upserted = view;
            return Task.CompletedTask;
        }
    }
}
