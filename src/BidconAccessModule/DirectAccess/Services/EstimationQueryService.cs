using BidconAccessModule.DirectAccess.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Dapper;
using BidconAccessModule.DirectAccess.Data.Abstractions;

namespace BidconAccessModule.DirectAccess.Services;
internal class EstimationQueryService : IEstimationQueryService
{
    private readonly IDbConnectionStringStore _connectionStringBuilder;
    public EstimationQueryService(IDbConnectionStringStore connectionStringBuilder)
    {
        _connectionStringBuilder = connectionStringBuilder;
    }

    public async Task<EstimationBatch> GetEstimationBatchAsync(string estimationId, string tenantId)
    {
        var sql = @"
SELECT E.EstimationID, E.Name, E.Description, E.Customer, E.Place, E.HandlingOfficer, E.ConfirmationOfficer, E.IsLocked, E.FolderNum, EV.EstCurrency as Currency, EV.ObjectFactor, EV.TenderTotal, EV.TenderType, EV.State as EstimationState FROM Estimation AS E LEFT JOIN EstimationVersion AS EV ON E.EstimationID = EV.EstimationID and EV.Version = E.CurrentVersion WHERE E.EstimationId = @Id;
SELECT EstimationID, LayerID, RowNum as Row, FatherRowNum as ParentRow, RowDescription as Description, Remark, Quantity, Unit, RowType, SheetType, LayerType, RevisionCode, Position, PMATANum, AddedInPhase, UnitPriceManual FROM EstimationSheet WHERE EstimationID = @Id AND Active = 1 AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
SELECT ID, EstimationID, LayerID, Cons, LayerType FROM MELayer WHERE EstimationID = @Id AND IsActive = 1 AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
SELECT ID, EstimationID, LayerID, Cons FROM DELayer WHERE EstimationID = @Id AND IsActive = 1 AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
SELECT ID, EstimationID, LayerID, Cons, ConsFactor, Waste FROM PRLayer WHERE EstimationID = @Id AND IsActive = 1 AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
SELECT ID, EstimationID, Description, ResourceType, Price FROM Resource WHERE EstimationID = @Id AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
SELECT EstimationID, ResourceType, Factor FROM ResourceFactors WHERE EstimationID = @Id AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
SELECT EstimationID, PMATANum, ID AS Name, Description FROM PM_ATA WHERE EstimationID = @Id AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
SELECT EstimationID, PMATANum, ResourceType, RemovalPer as RemovealPercent, RemovalExpensePer AS RemovalExpensePercent, AdditionalPer AS AdditionalPercent, AdditionalExpensePer AS AdditionalExpensePercent FROM PM_ATAFactor WHERE EstimationID = @Id AND Version = (SELECT CurrentVersion FROM Estimation WHERE EstimationID = @Id);
";
        using (IDbConnection cnn = new SqlConnection(await _connectionStringBuilder.BuildAsync(tenantId)))
        {
            using (var multi = await cnn.QueryMultipleAsync(sql, new { Id = estimationId.ToString() }))
            {
                var estimation = await multi.ReadFirstAsync<B_Estimation>();
                var sheets = await multi.ReadAsync<EstimationSheet>();
                var mixedLayers = await multi.ReadAsync<MixedElementLayer>();
                var designElementLayers = await multi.ReadAsync<DesignElementLayer>();
                var workResultLayers = await multi.ReadAsync<WorkResultLayer>();
                var resources = await multi.ReadAsync<Resource>();
                var resourceFactors = await multi.ReadAsync<ResourceFactor>();
                var ataResults = await multi.ReadAsync<ATA>();
                var ataFactorResults = await multi.ReadAsync<ATAFactor>();
                return new EstimationBatch(
                    estimation,
                    sheets.ToList(),
                    mixedLayers.ToList(),
                    designElementLayers.ToList(),
                    workResultLayers.ToList(),
                    resources.ToList(),
                    resourceFactors.ToList(),
                    ataResults.ToList(),
                    ataFactorResults.ToList());
            }
        }
    }
    public async Task<IEnumerable<EstimationBatch>> GetEstimationBatchesAsync(IEnumerable<string> estimationIds, string tenantId)
    {
        var sql = @"
SELECT E.EstimationID, E.Name, E.Description, E.Customer, E.Place, E.HandlingOfficer, E.ConfirmationOfficer, E.IsLocked, E.FolderNum, EV.EstCurrency as Currency, EV.ObjectFactor, EV.TenderTotal, EV.TenderType, EV.State as EstimationState FROM Estimation AS E LEFT JOIN EstimationVersion AS EV ON E.EstimationID = EV.EstimationID and EV.Version = E.CurrentVersion WHERE E.EstimationId IN @Ids;
SELECT EstimationID, LayerID, RowNum as Row, FatherRowNum as ParentRow, RowDescription as Description, Remark, Quantity, Unit, RowType, SheetType, LayerType, RevisionCode, Position, PMATANum, AddedInPhase, UnitPriceManual FROM EstimationSheet WHERE EstimationID IN @Ids AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = EstimationSheet.EstimationID);
SELECT ID, EstimationID, LayerID, Cons, LayerType FROM MELayer WHERE EstimationID IN @Ids AND IsActive = 1 AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = MELayer.EstimationID);
SELECT ID, EstimationID, LayerID, Cons FROM DELayer WHERE EstimationID IN @Ids AND IsActive = 1 AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = DELayer.EstimationID);
SELECT ID, EstimationID, LayerID, Cons, ConsFactor, Waste FROM PRLayer WHERE EstimationID IN @Ids AND IsActive = 1 AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = PRLayer.EstimationID);
SELECT ID, EstimationID, Description, ResourceType, Price FROM Resource WHERE EstimationID IN @Ids AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = Resource.EstimationID);
SELECT EstimationID, ResourceType, Factor FROM ResourceFactors WHERE EstimationID IN @Ids AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = ResourceFactors.EstimationId);
SELECT EstimationID, PMATANum, ID AS Name, Description FROM PM_ATA WHERE EstimationID IN @Ids AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = PM_ATA.EstimationID);
SELECT EstimationID, PMATANum, ResourceType, RemovalPer as RemovealPercent, RemovalExpensePer AS RemovalExpensePercent, AdditionalPer AS AdditionalPercent, AdditionalExpensePer AS AdditionalExpensePercent FROM PM_ATAFactor WHERE EstimationID IN @Ids AND Version IN (SELECT CurrentVersion FROM Estimation WHERE EstimationID = PM_ATAFactor.EstimationID);
";
        using (IDbConnection cnn = new SqlConnection(await _connectionStringBuilder.BuildAsync(tenantId)))
        {
            using (var multi = await cnn.QueryMultipleAsync(sql, new { Ids = estimationIds }))
            {
                var estimationResults = await multi.ReadAsync<B_Estimation>();
                var sheetResults = await multi.ReadAsync<EstimationSheet>();
                var mixedLayerResults = await multi.ReadAsync<MixedElementLayer>();
                var designElementLayerResults = await multi.ReadAsync<DesignElementLayer>();
                var workResultLayerResults = await multi.ReadAsync<WorkResultLayer>();
                var resourceResults = await multi.ReadAsync<Resource>();
                var resourceFactorsResults = await multi.ReadAsync<ResourceFactor>();
                var ataResults = await multi.ReadAsync<ATA>();
                var ataFactorResults = await multi.ReadAsync<ATAFactor>();

                var batches = new List<EstimationBatch>();
                var estimationResultsMap = estimationResults.ToLookup(er => er.EstimationID);
                foreach (var estimationId in estimationIds.Select(x => Guid.Parse(x)))
                {
                    var sheets = sheetResults.Where(sr => sr.EstimationId == estimationId).ToList();
                    var mixedLayers = mixedLayerResults.Where(mlr => mlr.EstimationId == estimationId).ToList();
                    var designElementLayers = designElementLayerResults.Where(delr => delr.EstimationId == estimationId).ToList();
                    var workResultLayers = workResultLayerResults.Where(wrlr => wrlr.EstimationId == estimationId).ToList();
                    var resources = resourceResults.Where(rr => rr.EstimationId == estimationId).ToList();
                    var resourceFactors = resourceFactorsResults.Where(rf => rf.EstimationId == estimationId).ToList();
                    var ata = ataResults.Where(rf => rf.EstimationId == estimationId).ToList();
                    var ataFactor = ataFactorResults.Where(rf => rf.EstimationId == estimationId).ToList();

                    var estimation = estimationResultsMap[estimationId].FirstOrDefault();
                    if (estimation is not null)
                    {
                        batches.Add(new EstimationBatch(
                        estimation,
                        sheets,
                        mixedLayers,
                        designElementLayers,
                        workResultLayers,
                        resources,
                        resourceFactors,
                        ata,
                        ataFactor));
                    }
                }
                return batches;
            }
        }
    }
    public async Task<IEnumerable<B_Estimation>> GetEstimationListAsync(string tenantId)
    {
        var sql = "SELECT EstimationID, Name, Description, Customer, Place, HandlingOfficer, ConfirmationOfficer, IsLocked, FolderNum, CurrentVersion FROM Estimation";
        using (IDbConnection cnn = new SqlConnection(await _connectionStringBuilder.BuildAsync(tenantId)))
        {
            return await cnn.QueryAsync<B_Estimation>(sql);
        }
    }
    public async Task<EstimationFolderBatch> GetFolderBatchAsync(string tenantId)
    {
        var sql = @"
SELECT EstimationID, Name, Description, Customer, Place, HandlingOfficer, ConfirmationOfficer, IsLocked, FolderNum, CurrentVersion FROM Estimation;
SELECT FolderNum, ParentNum, Name FROM EstimationFolder;
";
        using (IDbConnection cnn = new SqlConnection(await _connectionStringBuilder.BuildAsync(tenantId)))
        {
            using (var multi = await cnn.QueryMultipleAsync(sql))
            {
                var estimations = await multi.ReadAsync<B_Estimation>();
                var folders = await multi.ReadAsync<EstimationFolder>();
                return new EstimationFolderBatch(estimations, folders);
            }
        }
    }
}
