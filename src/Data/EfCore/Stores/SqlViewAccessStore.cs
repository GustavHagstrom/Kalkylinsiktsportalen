using Core.Models.EstimationView;
using Data.Abstractions;

namespace Data.EfCore.Stores;
public class SqlViewAccessStore : IViewAccessStore
{
    public Task<List<View>> GetAvailableViewsAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
