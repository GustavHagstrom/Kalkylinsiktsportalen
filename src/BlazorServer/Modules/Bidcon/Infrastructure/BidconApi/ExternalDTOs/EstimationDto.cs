namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

public class EstimationDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DisciplineTypeDto Discipline { get; set; }

    public WorksheetStatusTypeDto CurrentWorkSheet { get; set; }

    public WorksheetSubStatusTypeDto CurrentWorkSheetSubState { get; set; }

    public string Customer { get; set; } = string.Empty;

    public string HandlingOfficer { get; set; } = string.Empty;

    public Guid Id { get; set; }

    public string Location { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }
    
    public DateTime EditDate { get; set; }
    
    public DateTime InitiationDate { get; set; }

    public bool Reconciled { get; set; }
    
    public bool Reconciliation { get; set; }

    public string EstimationUri { get; set; } = string.Empty;

    public Guid ProjectId { get; set; }

    public string Link { get; set; } = string.Empty;
}
