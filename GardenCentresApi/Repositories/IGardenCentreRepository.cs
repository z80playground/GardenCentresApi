using GardenCentresApi.Dto;
using GardenCentresApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GardenCentresApi.Repositories
{
    public interface IGardenCentreRepository
    {
        Task<(List<GardenCentreDto>, int)> GetAllAsync(int page, int pageSize, string region);
        Task<GardenCentreDto> GetByIdAsync(int id, string region);
        Task<GardenCentreDto> GetByIdWithLocationAsync(int id, string region);
        Task AddAsync(GardenCentreDto gardenCentre);
        Task UpdateAsync(GardenCentreDto gardenCentre);
        Task DeleteAsync(int id);
    }
}