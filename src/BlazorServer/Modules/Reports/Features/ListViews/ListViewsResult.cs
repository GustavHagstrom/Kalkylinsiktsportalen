using BlazorServer.Modules.Reports.Public.Contracts.Views;

namespace BlazorServer.Modules.Reports.Features.ListViews;

public abstract record ListViewsResult;

public sealed record ViewsListed(IReadOnlyList<ViewDto> Views) : ListViewsResult;
