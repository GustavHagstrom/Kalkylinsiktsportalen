namespace BidconAccessModule.DirectAccess.Models;
internal record EstimationFolderBatch
    (IEnumerable<B_Estimation> Estimations, IEnumerable<EstimationFolder> Folders);