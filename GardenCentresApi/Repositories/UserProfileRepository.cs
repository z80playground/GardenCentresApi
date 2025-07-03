using GardenCentresApi.Data;
using GardenCentresApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GardenCentresApi.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly GardenCentreContext _context;

        public UserProfileRepository(GardenCentreContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserProfile> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
            }
            return await _context.UserProfiles
                .SingleOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task AddAsync(UserProfile userProfile)
        {
            if (userProfile == null)
            {
                throw new ArgumentNullException(nameof(userProfile));
            }
            if (string.IsNullOrWhiteSpace(userProfile.UserId))
            {
                throw new ArgumentException("UserProfile.UserId cannot be null or empty.", nameof(userProfile));
            }
            if (string.IsNullOrWhiteSpace(userProfile.Region) || (userProfile.Region != "UK" && userProfile.Region != "US"))
            {
                throw new ArgumentException("Region must be 'UK' or 'US'.", nameof(userProfile));
            }
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserProfile userProfile)
        {
            if (userProfile == null)
            {
                throw new ArgumentNullException(nameof(userProfile));
            }
            if (string.IsNullOrWhiteSpace(userProfile.UserId))
            {
                throw new ArgumentException("UserProfile.UserId cannot be null or empty.", nameof(userProfile));
            }
            if (string.IsNullOrWhiteSpace(userProfile.Region) || (userProfile.Region != "UK" && userProfile.Region != "US"))
            {
                throw new ArgumentException("Region must be 'UK' or 'US'.", nameof(userProfile));
            }
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
        }
    }
}