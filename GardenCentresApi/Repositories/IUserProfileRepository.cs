using GardenCentresApi.Models;
using System.Threading.Tasks;

namespace GardenCentresApi.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetByUserIdAsync(string userId);
        Task AddAsync(UserProfile userProfile);
        Task UpdateAsync(UserProfile userProfile);
    }
}