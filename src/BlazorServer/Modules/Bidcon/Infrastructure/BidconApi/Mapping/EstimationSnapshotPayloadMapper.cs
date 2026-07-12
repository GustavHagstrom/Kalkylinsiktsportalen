using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;
using DisciplineType = BlazorServer.Modules.Bidcon.Domain.DisciplineType;
using RowType = BlazorServer.Modules.Bidcon.Domain.RowType;
using SheetType = BlazorServer.Modules.Bidcon.Domain.SheetType;
using WorksheetStatusType = BlazorServer.Modules.Bidcon.Domain.WorksheetStatusType;
using WorksheetSubStatusType = BlazorServer.Modules.Bidcon.Domain.WorksheetSubStatusType;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Mapping;

internal static class EstimationSnapshotPayloadMapper
{
    public static EstimationSnapshotPayloadV1 ToSnapshotPayload(this EstimationDetailsDto dto)
    {
        return new EstimationSnapshotPayloadV1(
            dto.Name,
            dto.Description,
            (DisciplineType)dto.Discipline,
            (WorksheetStatusType)dto.CurrentWorkSheet,
            (WorksheetSubStatusType)dto.CurrentWorkSheetSubState,
            dto.Customer,
            dto.HandlingOfficer,
            dto.Id,
            dto.Location,
            dto.CreateDate,
            dto.EditDate,
            dto.InitiationDate,
            dto.Reconciled,
            dto.Reconciliation,
            dto.EstimationUri,
            dto.ProjectId,
            dto.Link,
            dto.BidPhaseResources.Select(ToSnapshotPayload).ToList(),
            dto.ProductionPhaseResources.Select(ToSnapshotPayload).ToList(),
            dto.Variations.Select(ToSnapshotPayload).ToList(),
            dto.BidPhaseStructures?.ToSnapshotPayload(),
            dto.VariationProductionPhaseStructures?.ToSnapshotPayload(),
            dto.DynamicProperties.Select(ToSnapshotPayload).ToList(),
            dto.TenderRateExVAT,
            dto.TenderRateIncVAT,
            dto.IsCompleted);
    }

    private static ResourceSnapshotPayload ToSnapshotPayload(ResourceDto dto)
    {
        return new ResourceSnapshotPayload(
            dto.AccountCode,
            dto.AccountDecription,
            dto.ResourceTypeCode,
            dto.ResourceTypeDescription,
            (SheetType)dto.SheetType,
            dto.DSCode,
            dto.DSCodeDescription,
            dto.EstimationCode,
            dto.Description,
            dto.LocationCode,
            dto.LocationDescription,
            dto.TradeCode,
            dto.TradeCodeDescription,
            dto.Cost,
            dto.CompletedCost,
            dto.Quantity,
            dto.CompletedQuantity,
            dto.Unit,
            dto.ArticleNumber,
            dto.ExternalId,
            dto.CO2Value,
            dto.Weight,
            dto.UnitNetPrice,
            dto.UnitCustomerPrice,
            dto.Waste,
            dto.BSAB96,
            dto.BSAB96Description,
            dto.BD96,
            dto.Remark,
            dto.BD96Description,
            dto.TimeListCode,
            dto.TimeListCodeDescription,
            dto.WorkResultDSCode,
            dto.WorkResultDescription,
            dto.WorkResultQuantity,
            dto.WorkResultUnit,
            dto.DesignElementBSAB96Code,
            dto.DesignElementDescription,
            dto.DesignElementQuantity,
            dto.DesignElementUnit,
            dto.MixedElementDescription,
            dto.MixedElementQuantity,
            dto.MixedElementUnit,
            dto.HierarchyLevel1Description,
            dto.HierarchyLevel2Description,
            dto.HierarchyLevel3Description);
    }

    private static VariationDetailsSnapshotPayload ToSnapshotPayload(VariationDetailsDto dto)
    {
        return new VariationDetailsSnapshotPayload(
            dto.AffectsCostBudget,
            dto.AffectsRevenueBudget,
            dto.ApprovedAmount,
            dto.Code,
            dto.Description,
            dto.RevenueBudgetAccount,
            dto.Resources.Select(ToSnapshotPayload).ToList());
    }

    private static DynamicPropertySnapshotPayload ToSnapshotPayload(DynamicPropertiesDto dto)
    {
        return new DynamicPropertySnapshotPayload(dto.Identifier, dto.Description, dto.Value);
    }

    private static StructureSnapshotPayload ToSnapshotPayload(this Structure dto)
    {
        return new StructureSnapshotPayload(
            dto.Accounts?.ToSnapshotPayload(),
            dto.ResourceTypes?.ToSnapshotPayload(),
            dto.DSCodes?.ToSnapshotPayload(),
            dto.Locations?.ToSnapshotPayload(),
            dto.Estimation?.ToSnapshotPayload(),
            dto.TradeCodes?.ToSnapshotPayload());
    }

    private static StructureCodeNodeSnapshotPayload ToSnapshotPayload(this StructureCodeNode dto)
    {
        return new StructureCodeNodeSnapshotPayload(
            dto.Code,
            dto.Description,
            dto.Children.Select(ToSnapshotPayload).ToList());
    }

    private static StructureEstimationSnapshotPayload ToSnapshotPayload(this StructureEstimationDto dto)
    {
        return new StructureEstimationSnapshotPayload(
            dto.None?.ToSnapshotPayload(),
            dto.NetSheet?.ToSnapshotPayload(),
            dto.TenderSheet?.ToSnapshotPayload(),
            dto.PrelimsSheet?.ToSnapshotPayload(),
            dto.SummarySheet?.ToSnapshotPayload(),
            dto.LockedStagesSheet?.ToSnapshotPayload(),
            dto.ReconciliationSheet?.ToSnapshotPayload());
    }

    private static StructureEstimationNodeSnapshotPayload ToSnapshotPayload(this StructureEstimationNodeDto dto)
    {
        return new StructureEstimationNodeSnapshotPayload(
            dto.Description,
            dto.Children.Select(ToSnapshotPayload).ToList(),
            dto.Code,
            dto.Remark,
            dto.RowType is null ? null : (RowType)dto.RowType,
            dto.RowOrder,
            dto.IsActive,
            dto.Quantity,
            dto.Unit,
            dto.Reference);
    }
}
