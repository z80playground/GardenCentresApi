using GardenCentresApi.Dto;
using GardenCentresApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GardenCentresApi.Repositories
{
    public interface ILocationRepository
    {
        Task<List<LocationDto>> GetAllAsync(string region);
        Task<LocationDto> GetByIdAsync(int id, string region);
        Task<List<GardenCentreDto>> GetGardenCentresByLocationAsync(int locationId, string region);
        Task<LocationDto> AddAsync(CreateLocationDto location);
        Task UpdateAsync(int id, UpdateLocationDto location);
        Task DeleteAsync(int id);
    }
}