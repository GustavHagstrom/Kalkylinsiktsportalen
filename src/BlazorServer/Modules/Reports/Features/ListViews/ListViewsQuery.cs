using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Reports.Features.ListViews;

public sealed record ListViewsQuery(OrganizationId OrganizationId) : IQuery<ListViewsResult>;
