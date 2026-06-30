namespace BlazorServer.Modules.Bidcon.Domain;

public sealed record EstimationSnapshotPayloadV1(
    string Name,
    string Description,
    DisciplineType Discipline,
    WorksheetStatusType CurrentWorkSheet,
    WorksheetSubStatusType CurrentWorkSheetSubState,
    string Customer,
    string HandlingOfficer,
    Guid Id,
    string Location,
    DateTime CreateDate,
    DateTime EditDate,
    DateTime InitiationDate,
    bool Reconciled,
    bool Reconciliation,
    string EstimationUri,
    Guid ProjectId,
    string Link,
    IReadOnlyList<ResourceSnapshotPayload> BidPhaseResources,
    IReadOnlyList<ResourceSnapshotPayload> ProductionPhaseResources,
    IReadOnlyList<VariationDetailsSnapshotPayload> Variations,
    StructureSnapshotPayload? BidPhaseStructures,
    StructureSnapshotPayload? VariationProductionPhaseStructures,
    IReadOnlyList<DynamicPropertySnapshotPayload> DynamicProperties,
    double TenderRateExVAT,
    double TenderRateIncVAT,
    bool IsCompleted);

public sealed record ResourceSnapshotPayload(
    string AccountCode,
    string AccountDecription,
    string ResourceTypeCode,
    string ResourceTypeDescription,
    SheetType SheetType,
    string DSCode,
    string DSCodeDescription,
    string EstimationCode,
    string Description,
    string LocationCode,
    string LocationDescription,
    string TradeCode,
    string TradeCodeDescription,
    decimal Cost,
    decimal CompletedCost,
    decimal Quantity,
    decimal CompletedQuantity,
    string Unit,
    string ArticleNumber,
    string ExternalId,
    decimal CO2Value,
    decimal Weight,
    decimal? UnitNetPrice,
    decimal? UnitCustomerPrice,
    decimal? Waste,
    string BSAB96,
    string BSAB96Description,
    string BD96,
    string Remark,
    string BD96Description,
    string TimeListCode,
    string TimeListCodeDescription,
    string WorkResultDSCode,
    string WorkResultDescription,
    decimal WorkResultQuantity,
    string WorkResultUnit,
    string DesignElementBSAB96Code,
    string DesignElementDescription,
    decimal DesignElementQuantity,
    string DesignElementUnit,
    string MixedElementDescription,
    decimal MixedElementQuantity,
    string MixedElementUnit,
    string HierarchyLevel1Description,
    string HierarchyLevel2Description,
    string HierarchyLevel3Description);

public sealed record VariationDetailsSnapshotPayload(
    bool AffectsCostBudget,
    bool AffectsRevenueBudget,
    double? ApprovedAmount,
    string Code,
    string Description,
    string RevenueBudgetAccount,
    IReadOnlyList<ResourceSnapshotPayload> Resources);

public sealed record DynamicPropertySnapshotPayload(
    string Identifier,
    string Description,
    string Value);

public sealed record StructureSnapshotPayload(
    StructureCodeNodeSnapshotPayload? Accounts,
    StructureCodeNodeSnapshotPayload? ResourceTypes,
    StructureCodeNodeSnapshotPayload? DSCodes,
    StructureCodeNodeSnapshotPayload? Locations,
    StructureEstimationSnapshotPayload? Estimation,
    StructureCodeNodeSnapshotPayload? TradeCodes);

public sealed record StructureCodeNodeSnapshotPayload(
    string Code,
    string Description,
    IReadOnlyList<StructureCodeNodeSnapshotPayload> Children);

public sealed record StructureEstimationSnapshotPayload(
    StructureEstimationNodeSnapshotPayload? None,
    StructureEstimationNodeSnapshotPayload? NetSheet,
    StructureEstimationNodeSnapshotPayload? TenderSheet,
    StructureEstimationNodeSnapshotPayload? PrelimsSheet,
    StructureEstimationNodeSnapshotPayload? SummarySheet,
    StructureEstimationNodeSnapshotPayload? LockedStagesSheet,
    StructureEstimationNodeSnapshotPayload? ReconciliationSheet);

public sealed record StructureEstimationNodeSnapshotPayload(
    string Description,
    IReadOnlyList<StructureEstimationNodeSnapshotPayload> Children,
    string Code,
    string Remark,
    RowType? RowType,
    int RowOrder,
    bool IsActive,
    decimal Quantity,
    string Unit,
    string Reference);

public enum DisciplineType
{
    None = 0,
    Bygg = 1,
    El = 2,
    Vs = 4,
    Anl = 8,
    Vent = 16,
    Gbr = 32,
    Nzl = 33,
    Aus = 34,
    Fin = 35,
    ElKraft = 36,
    Sgp = 37,
    Eur = 40,
    Ned = 41,
    Esp = 42
}

public enum WorksheetStatusType
{
    None = 0,
    Bid = 1,
    Production = 3
}

public enum WorksheetSubStatusType
{
    None = 0,
    Bid = 1,
    Contract = 2,
    Production = 3
}

public enum SheetType
{
    None = 0,
    NetSheet = 1,
    TenderSheet = 2,
    PrelimsSheet = 3,
    SummarySheet = 14,
    LockedStagesSheet = 15,
    ReconciliationSheet = 18
}

public enum RowType
{
    None = 0,
    Group = 100,
    Part = 101,
    Posting = 102,
    MELayer = 103,
    DELayer = 104,
    PRLayer = 105,
    QuantityPosting = 106,
    LockedStages = 107,
    Text = 108,
    Invoice = 109
}
