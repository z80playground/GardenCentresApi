using GardenCentresApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GardenCentresApi.Repositories
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync(string region);
        Task<Location> GetByIdAsync(int id, string region);
        Task<List<GardenCentre>> GetGardenCentresByLocationAsync(int locationId, string region);
        Task AddAsync(Location location);
        Task UpdateAsync(Location location);
        Task DeleteAsync(int id);
    }
}