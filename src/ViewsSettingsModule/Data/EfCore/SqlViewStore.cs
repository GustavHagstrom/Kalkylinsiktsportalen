using Core.Models.EstimationView;
using Microsoft.EntityFrameworkCore;
using Data.EfCore;
using ViewSettingsModule.Data.Abstractions;

namespace ViewSettingsModule.Data.EfCore;
internal class SqlViewStore(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IViewStore
{


    public async Task DeleteAsync(string tenantId, View view)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var viewToDelete = await dbContext.Views.FirstOrDefaultAsync(x => x.Id == view.Id && x.TenantId == tenantId);
        if (viewToDelete is null)
        {
            throw new Exception("View not found");
        }
        dbContext.Views.Remove(viewToDelete);
        await dbContext.SaveChangesAsync();
    }

    public async Task<View?> GetAsync(string tenantId, string viewId)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var view = await dbContext.Views
            .Include(x => x.DataSections).ThenInclude(x => x.Columns)
            .Include(x => x.DataSections).ThenInclude(x => x.Rows)
            .Include(x => x.DataSections).ThenInclude(x => x.Cells).ThenInclude(x => x.CellFormat)
            .Include(x => x.SheetSections).ThenInclude(x => x.CellFormats)
            .Include(x => x.SheetSections).ThenInclude(x => x.Columns)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == viewId && x.TenantId == tenantId);
        return view;
    }

    public async Task<List<View>> GetViewListAsync(string tenantId)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Views
            .Where(x => x.TenantId == tenantId)
            .ToListAsync();
    }
    public async Task UpsertAsync(string tenantId, View view)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        view.TenantId = tenantId;

        //Adds the view to the database if none is found with the same Id
        var existingView = await dbContext.Views.FirstOrDefaultAsync(x => x.Id == view.Id);
        if (existingView is null)
        {
            await dbContext.Views.AddAsync(view);
        }
        else
        {
            dbContext.Entry(existingView).CurrentValues.SetValues(view);

            var existingDataSections = await dbContext.Set<DataSection>().Where(x => x.ViewId == view.Id).ToListAsync();
            foreach (var dataSection in EntetiesToDelete(existingDataSections, view.DataSections))
            {
                dbContext.Remove(dataSection);
            }
            foreach (var dataSection in EntetiesToAdd(existingDataSections, view.DataSections))
            {
                existingView.DataSections.Add(dataSection);
            }
            foreach (var dataSectionUpdateRecord in EntetiesToUpdate(existingDataSections, view.DataSections))
            {
                dbContext.Entry(dataSectionUpdateRecord.Existing).CurrentValues.SetValues(dataSectionUpdateRecord.Incoming);

                var existingRows = await dbContext.Set<DataRow>().Where(x => x.DataSectionId == dataSectionUpdateRecord.Incoming.Id).ToListAsync();
                foreach (var row in EntetiesToDelete(existingRows, dataSectionUpdateRecord.Incoming.Rows))
                {
                    dbContext.Remove(row);
                }
                foreach (var row in EntetiesToAdd(existingRows, dataSectionUpdateRecord.Incoming.Rows))
                {
                    existingRows.Add(row);
                }
                foreach (var rowUpdatePair in EntetiesToUpdate(existingRows, dataSectionUpdateRecord.Incoming.Rows))
                {
                    dbContext.Entry(rowUpdatePair.Existing).CurrentValues.SetValues(rowUpdatePair.Incoming);
                }

                var existingColumns = await dbContext.Set<DataColumn>().Where(x => x.DataSectionId == dataSectionUpdateRecord.Incoming.Id).ToListAsync();
                foreach (var column in EntetiesToDelete(existingColumns, dataSectionUpdateRecord.Incoming.Columns))
                {
                    dbContext.Remove(column);
                }
                foreach (var column in EntetiesToAdd(existingColumns, dataSectionUpdateRecord.Incoming.Columns))
                {
                    existingColumns.Add(column);
                }
                foreach (var columnUpdateRecord in EntetiesToUpdate(existingColumns, dataSectionUpdateRecord.Incoming.Columns))
                {
                    dbContext.Entry(columnUpdateRecord.Existing).CurrentValues.SetValues(columnUpdateRecord.Incoming);
                }

                var existingDataCells = await dbContext.Set<DataCell>().Where(x => x.DataSectionId == dataSectionUpdateRecord.Incoming.Id).Include(x => x.CellFormat).ToListAsync();
                foreach (var cell in EntetiesToDelete(existingDataCells, dataSectionUpdateRecord.Incoming.Cells))
                {
                    dbContext.Remove(cell);
                }
                foreach (var cell in EntetiesToAdd(existingDataCells, dataSectionUpdateRecord.Incoming.Cells))
                {
                    existingDataCells.Add(cell);
                }
                foreach (var cellUpdateRecord in EntetiesToUpdate(existingDataCells, dataSectionUpdateRecord.Incoming.Cells))
                {
                    dbContext.Entry(cellUpdateRecord.Existing).CurrentValues.SetValues(cellUpdateRecord.Incoming);
                    dbContext.Entry(cellUpdateRecord.Existing.CellFormat!).CurrentValues.SetValues(cellUpdateRecord.Incoming.CellFormat!);
                }
            }

            var existingSheetSections = await dbContext.Set<SheetSection>().Where(x => x.ViewId == view.Id).ToListAsync();
            foreach (var sheetSection in EntetiesToDelete(existingSheetSections, view.SheetSections))
            {
                dbContext.Remove(sheetSection);
            }
            foreach (var sheetSection in EntetiesToAdd(existingSheetSections, view.SheetSections))
            {
                existingView.SheetSections.Add(sheetSection);
            }
            foreach (var sheetSectionUpdateRecord in EntetiesToUpdate(existingSheetSections, view.SheetSections))
            {
                dbContext.Entry(sheetSectionUpdateRecord.Existing).CurrentValues.SetValues(sheetSectionUpdateRecord.Incoming);

                var existingColumns = await dbContext.Set<SheetColumn>().Where(x => x.SheetSectionId == sheetSectionUpdateRecord.Incoming.Id).ToListAsync();
                foreach (var row in EntetiesToDelete(existingColumns, sheetSectionUpdateRecord.Incoming.Columns))
                {
                    dbContext.Remove(row);
                }
                foreach (var row in EntetiesToAdd(existingColumns, sheetSectionUpdateRecord.Incoming.Columns))
                {
                    existingColumns.Add(row);
                }
                foreach (var columnUpdateRecord in EntetiesToUpdate(existingColumns, sheetSectionUpdateRecord.Incoming.Columns))
                {
                    dbContext.Entry(columnUpdateRecord.Existing).CurrentValues.SetValues(columnUpdateRecord.Incoming);
                }

                var existingCellFormats = await dbContext.Set<SheetCellFormat>().Where(x => x.SheetSectionId == sheetSectionUpdateRecord.Incoming.Id).ToListAsync();
                foreach (var cellFormat in EntetiesToDelete(existingCellFormats, sheetSectionUpdateRecord.Incoming.CellFormats))
                {
                    dbContext.Remove(cellFormat);
                }
                foreach (var cellFormat in EntetiesToAdd(existingCellFormats, sheetSectionUpdateRecord.Incoming.CellFormats))
                {
                    existingCellFormats.Add(cellFormat);
                }
                foreach (var cellFormatUpdateRecord in EntetiesToUpdate(existingCellFormats, sheetSectionUpdateRecord.Incoming.CellFormats))
                {
                    dbContext.Entry(cellFormatUpdateRecord.Existing).CurrentValues.SetValues(cellFormatUpdateRecord.Incoming);
                }
            }

            var existingTags = await dbContext.Set<Tag>().Where(x => x.ViewId == view.Id).ToListAsync();
            foreach (var tag in EntetiesToDelete(existingTags, view.Tags))
            {
                dbContext.Remove(tag);
            }
            foreach (var tag in EntetiesToAdd(existingTags, view.Tags))
            {
                existingView.Tags.Add(tag);
            }
            foreach (var tagUpdateRecord in EntetiesToUpdate(existingTags, view.Tags))
            {
                dbContext.Entry(tagUpdateRecord.Existing).CurrentValues.SetValues(tagUpdateRecord.Incoming);
            }

            //var eDS = await dbContext.Set<DataSection>().Where(x => x.ViewId == view.Id).ToListAsync();
            //await RemoveAddUpdateViewEnteties(dbContext, eDS, view.DataSections, async (dsUpdatePair) =>
            //{
            //    var existingRows = await dbContext.Set<DataRow>().Where(x => x.DataSectionId == dsUpdatePair.Incoming.Id).ToListAsync();
            //    await RemoveAddUpdateViewEnteties(dbContext, existingRows, dsUpdatePair.Incoming.Rows);
            //    var existingColumns = await dbContext.Set<DataColumn>().Where(x => x.DataSectionId == dsUpdatePair.Incoming.Id).ToListAsync();
            //    await RemoveAddUpdateViewEnteties(dbContext, existingColumns, dsUpdatePair.Incoming.Columns);
            //    var existingDataCells = await dbContext.Set<DataCell>().Where(x => x.DataSectionId == dsUpdatePair.Incoming.Id).Include(x => x.CellFormat).ToListAsync();
            //    await RemoveAddUpdateViewEnteties(dbContext, existingDataCells, dsUpdatePair.Incoming.Cells, async (cellUpdatePair) =>
            //    {
            //        dbContext.Entry(cellUpdatePair.Existing.CellFormat!).CurrentValues.SetValues(cellUpdatePair.Incoming.CellFormat!);
            //        await Task.CompletedTask;
            //    });
            //    await dbContext.SaveChangesAsync();
            //});

            //var eSS = await dbContext.Set<SheetSection>().Where(x => x.ViewId == view.Id).ToListAsync();
            //await RemoveAddUpdateViewEnteties(dbContext, eSS, view.SheetSections, async (ssUpdatePair) =>
            //{
            //    var existingColumns = await dbContext.Set<SheetColumn>().Where(x => x.SheetSectionId == ssUpdatePair.Incoming.Id).ToListAsync();
            //    await RemoveAddUpdateViewEnteties(dbContext, existingColumns, ssUpdatePair.Incoming.Columns);
            //    var existingCellFormats = await dbContext.Set<SheetCellFormat>().Where(x => x.SheetSectionId == ssUpdatePair.Incoming.Id).ToListAsync();
            //    await RemoveAddUpdateViewEnteties(dbContext, existingCellFormats, ssUpdatePair.Incoming.CellFormats);
            //    await dbContext.SaveChangesAsync();
            //});

            //var eTags = await dbContext.Set<Tag>().Where(x => x.ViewId == view.Id).ToListAsync();
            //await RemoveAddUpdateViewEnteties(dbContext, eTags, view.Tags);
        }
        await dbContext.SaveChangesAsync();
    }
    private async Task RemoveAddUpdateViewEnteties<T>(ApplicationDbContext dbContext, List<T> existing, List<T> incoming, Func<UpdatePair<T>, Task>? extraActionOnUpdate = null) where T : class, IViewEntity
    {
        foreach (var entety in EntetiesToDelete(existing, incoming))
        {
            dbContext.Remove(entety);
        }
        foreach (var entety in EntetiesToAdd(existing, incoming))
        {
            existing.Add(entety);
        }
        foreach (var updatePair in EntetiesToUpdate(existing, incoming))
        {
            dbContext.Entry(updatePair.Existing).CurrentValues.SetValues(updatePair.Incoming);
            if (extraActionOnUpdate is not null)
            {
                await extraActionOnUpdate.Invoke(updatePair);
            }
        }

    }
    private IEnumerable<UpdatePair<T>> EntetiesToUpdate<T>(IEnumerable<T> existingEnteties, IEnumerable<T> incomingEnteties) where T : IViewEntity
    {
        foreach (var existing in existingEnteties)
        {
            var incomingMatch = incomingEnteties.FirstOrDefault(x => x.Id == existing.Id);
            if (incomingMatch is not null)
            {
                yield return new UpdatePair<T>(existing, incomingMatch);
            }
        }
    }
    private IEnumerable<T> EntetiesToDelete<T>(IEnumerable<T> existingEnteties, IEnumerable<T> incomingEnteties) where T : IViewEntity
    {
        return existingEnteties.Where(x => !incomingEnteties.Any(n => n.Id == x.Id));
    }
    private IEnumerable<T> EntetiesToAdd<T>(IEnumerable<T> existingEnteties, IEnumerable<T> incomingEnteties) where T : IViewEntity
    {
        return incomingEnteties.Where(n => !existingEnteties.Any(x => x.Id == n.Id));
    }

    private record UpdatePair<T>(T Existing, T Incoming);
}
