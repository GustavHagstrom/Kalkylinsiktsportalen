using Core.Models.EstimationEnteties;
using Core.Abstractions;
using BidCon.SDK.Database;
using System.Security.Claims;
using Core.Models.FromShared;
using BidconAccessModule.SdkAccess.Utilities;

namespace BidconAccessModule.SdkAccess.Services;
internal class BidconSdkReflectionAccess(IBidconReflectionService ReflectionService) : IBidconAccess
{
    private DatabaseUser? _user = null;

    private Task<DatabaseUser> LazyUserAsync(string tenantId)
    {
        if (_user is not null) return Task.FromResult(_user);
        _user = ReflectionService.CreateUser();
        return Task.FromResult(_user);
    }


    public async Task<Estimation> GetEstimationAsync(string estimationId, string tenantId)
    {
        var user = await LazyUserAsync(tenantId);
        var bEstimation = user.ReadEstimation(estimationId);
        var estimation = EstimationModelBuilder.Build(bEstimation);
        return estimation;
    }

    public async Task<Folder> GetFolderRootAsync(string tenantId)
    {
        var user = await LazyUserAsync(tenantId);
        var dbFolder = await Task.Run(() => user!.ReadEstimations());
        var folder = CreateFolder(dbFolder);
        return folder;
    }

    private Folder CreateFolder(DatabaseFolder root)
    {
        var folder = new Folder
        {
            FolderNum = root.FolderNum,
            ParentNum = root.Parent is null ? -2 : root.Parent.FolderNum,
            Name = root.Name ?? string.Empty,
            DbEstimations = new List<EstimationInfo>(),
            SubFolders = new List<Folder>()
        };
        foreach (var estimation in root.Estimations)
        {
            folder.DbEstimations.Add(CreateEstimationInfo(estimation, folder.FolderNum));
        }
        foreach (var subFolder in root.Folders)
        {
            folder.SubFolders.Add(CreateFolder(subFolder));
        }
        return folder;
    }
    private EstimationInfo CreateEstimationInfo(DatabaseEstimation dbEstimation, int folderNum)
    {
        return new EstimationInfo
        {
            FolderNum = folderNum,
            Name = dbEstimation.Name,
            Description = dbEstimation.Description,
            Id = dbEstimation.ID,
        };
    }
    
}
