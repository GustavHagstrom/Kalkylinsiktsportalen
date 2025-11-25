using Core.Models.EstimationView;

namespace Data.Abstractions;
public interface IViewAccessStore
{
    Task<List<View>> GetAvailableViewsAsync(string userId);
}
