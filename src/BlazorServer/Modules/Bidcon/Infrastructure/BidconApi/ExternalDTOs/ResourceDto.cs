namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

public class ResourceDto
{
    public string AccountCode { get; set; } = string.Empty;

    public string AccountDecription { get; set; } = string.Empty;

    public string ResourceTypeCode { get; set; } = string.Empty;

    public string ResourceTypeDescription { get; set; } = string.Empty;

    public SheetTypeDto SheetType { get; set; }

    public string DSCode { get; set; } = string.Empty;

    public string DSCodeDescription { get; set; } = string.Empty;

    public string EstimationCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string LocationCode { get; set; } = string.Empty;

    public string LocationDescription { get; set; } = string.Empty;

    public string TradeCode { get; set; } = string.Empty;

    public string TradeCodeDescription { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public decimal CompletedCost { get; set; }

    public decimal Quantity { get; set; }

    public decimal CompletedQuantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public string ArticleNumber { get; set; } = string.Empty;

    public string ExternalId { get; set; } = string.Empty;

    public decimal CO2Value { get; set; }

    public decimal Weight { get; set; }

    public decimal? UnitNetPrice { get; set; }

    public decimal? UnitCustomerPrice { get; set; }

    public decimal? Waste { get; set; }

    public string BSAB96 { get; set; } = string.Empty;

    public string BSAB96Description { get; set; } = string.Empty;

    public string BD96 { get; set; } = string.Empty;

    public string Remark { get; set; } = string.Empty;

    public string BD96Description { get; set; } = string.Empty;

    public string TimeListCode { get; set; } = string.Empty;

    public string TimeListCodeDescription { get; set; } = string.Empty;

    public string WorkResultDSCode { get; set; } = string.Empty;

    public string WorkResultDescription { get; set; } = string.Empty;

    public decimal WorkResultQuantity { get; set; }

    public string WorkResultUnit { get; set; } = string.Empty;

    public string DesignElementBSAB96Code { get; set; } = string.Empty;

    public string DesignElementDescription { get; set; } = string.Empty;

    public decimal DesignElementQuantity { get; set; }

    public string DesignElementUnit { get; set; } = string.Empty;

    public string MixedElementDescription { get; set; } = string.Empty;

    public decimal MixedElementQuantity { get; set; }

    public string MixedElementUnit { get; set; } = string.Empty;

    public string HierarchyLevel1Description { get; set; } = string.Empty;

    public string HierarchyLevel2Description { get; set; } = string.Empty;

    public string HierarchyLevel3Description { get; set; } = string.Empty;
}
