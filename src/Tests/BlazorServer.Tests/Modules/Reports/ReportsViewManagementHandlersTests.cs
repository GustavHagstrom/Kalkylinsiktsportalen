using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Features.CreateView;
using BlazorServer.Modules.Reports.Features.GetView;
using BlazorServer.Modules.Reports.Features.ListViews;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Views;

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
