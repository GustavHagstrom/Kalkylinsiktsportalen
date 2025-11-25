namespace BidconAccessModule.DirectAccess.Models;
internal class EstimationFolder

{
    public int FolderNum { get; set; }
    public int ParentNum { get; set; }
    public required string Name { get; set; }
    public override string ToString() => Name;
}
