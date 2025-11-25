namespace BidconAccessModule.DirectAccess.Models;
internal class DesignElementLayer
{
    public required string Id { get; set; }
    public Guid EstimationId { get; set; }
    public required string LayerId { get; set; }
    /// <summary>
    /// Quantity
    /// </summary>
    public double Cons { get; set; }
}
