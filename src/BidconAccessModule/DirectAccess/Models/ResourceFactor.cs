namespace BidconAccessModule.DirectAccess.Models;
internal class ResourceFactor
{
    public Guid EstimationId { get; set; }
    public required int ResourceType { get; set; }
    public required double Factor { get; set; }
}
