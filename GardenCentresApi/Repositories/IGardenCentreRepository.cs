using GardenCentresApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GardenCentresApi.Repositories
{
    public interface IGardenCentreRepository
    {
        Task<(List<GardenCentre>, int)> GetAllAsync(int page, int pageSize, string region);
        Task<GardenCentre> GetByIdAsync(int id, string region);
        Task<GardenCentre> GetByIdWithLocationAsync(int id, string region);
        Task AddAsync(GardenCentre gardenCentre);
        Task UpdateAsync(GardenCentre gardenCentre);
        Task DeleteAsync(int id);
    }
}