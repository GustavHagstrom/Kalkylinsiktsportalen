using AppPlatform.Core.Enums.BidconAccess;
using AppPlatform.Core.Models.EstimationEnteties;

namespace AppPlatform.BidconAccessModule.SdkAccess.Utilities;
internal static class EstimationModelBuilder
{
    public static Estimation Build(BidCon.SDK.Estimation bcEstimation)
    {
        var estimation = new Estimation
        {
            Name = bcEstimation.Name,
            BidconId = bcEstimation.ID.ToString(),
            Description = bcEstimation.Description,
            Customer = bcEstimation.Customer,
            Location = bcEstimation.Location,
            HandlingOfficer = bcEstimation.HandlingOfficer,
            ConfirmationOfficer = bcEstimation.ConfirmationOfficer,
            TenderTotal = bcEstimation.BidPrice,
            NetSheet = BuildEstimationSheet(bcEstimation.NetSheet),
            Resources = BuildResources(bcEstimation.ResourceSummary).ToList(),
        };
        return estimation;
    }

    private static IEnumerable<Resource> BuildResources(BidCon.SDK.Resource[] bcResourceSummary)
    {
        foreach (var item in bcResourceSummary)
        {
            yield return new Resource
            {
                Name = item.Name,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost ?? 0,
                Account = item.Account is null ? null : item.Account.ToString(),
                ResourceType = item.ResourceType.ToString(),
            };
        }
    }

    private static SheetItem BuildEstimationSheet(BidCon.SDK.EstimationSheet bcNetSheet)
    {
        int position = 0;
        return BuildSheetItem(bcNetSheet, ref position);
    }
    private static SheetItem BuildSheetItem(BidCon.SDK.EstimationItem bcItem, ref int position, SheetItem? parent = null)
    {
        var sheetItem = new SheetItem
        {
            Description = bcItem.Name,
            EstimationId = bcItem.ID,
            Type = GetRowType(bcItem.ItemType),
            Parent = parent,
            Position = position++,
            Quantity = bcItem.Quantity,
            Remark = bcItem.Remark,
            Unit = bcItem.Unit,
        };
        if(sheetItem.Type == RowType.CostBearer)
        {
            sheetItem.CostBearerUnitCost = bcItem.UnitCost;
            return sheetItem;
        }
        if(sheetItem.Type == RowType.Group || sheetItem.Type == RowType.Part)
        {
            foreach (var item in bcItem.Items)
            {
                if (item.IsActive)
                {
                    sheetItem.Children.Add(BuildSheetItem(item, ref position, sheetItem));
                }
            }
        }
        return sheetItem;
    }
    private static RowType GetRowType(BidCon.SDK.EstimationItemType bcType)
    {
        return bcType switch
        {
            BidCon.SDK.EstimationItemType.NetSheet => RowType.Group,
            BidCon.SDK.EstimationItemType.TenderSheet => RowType.Group,
            BidCon.SDK.EstimationItemType.SummarySheet => RowType.Group,
            BidCon.SDK.EstimationItemType.LockedStagesSheet => RowType.Group,
            BidCon.SDK.EstimationItemType.ReconciliationSheet => RowType.Group,
            BidCon.SDK.EstimationItemType.PrelimsSheet => RowType.Group,

            BidCon.SDK.EstimationItemType.Group => RowType.Group,
            BidCon.SDK.EstimationItemType.Part => RowType.Part,
            BidCon.SDK.EstimationItemType.MixedElement => RowType.CostBearer,
            BidCon.SDK.EstimationItemType.DesignElement => RowType.CostBearer,
            BidCon.SDK.EstimationItemType.WorkResult => RowType.CostBearer,
            
            BidCon.SDK.EstimationItemType.QuantityPosting => RowType.QuantityItem,
            BidCon.SDK.EstimationItemType.Text => RowType.Text,

            _ => RowType.None
        };
    }
}

