using GardenCentresApi.Data;
using GardenCentresApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GardenCentresApi.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly GardenCentreContext _context;
        private readonly string _region;

        public LocationRepository(GardenCentreContext context, string region)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(region) || (region != "UK" && region != "US"))
            {
                throw new ArgumentException("Region must be 'UK' or 'US'.", nameof(region));
            }
            _region = region;
        }

        public async Task<List<Location>> GetAllAsync(string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.Locations
                .Where(l => l.Region == _region)
                .ToListAsync();
        }

        public async Task<Location> GetByIdAsync(int id, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.Locations
                .Where(l => l.Id == id && l.Region == _region)
                .SingleOrDefaultAsync();
        }

        public async Task<List<GardenCentre>> GetGardenCentresByLocationAsync(int locationId, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.GardenCentres
                .Where(gc => gc.LocationId == locationId && gc.Region == _region)
                .Include(gc => gc.Location)
                .ToListAsync();
        }

        public async Task AddAsync(Location location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }
            if (location.Region != _region)
            {
                throw new ArgumentException($"Location region must match repository region: {_region}.", nameof(location));
            }
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Location location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }
            if (location.Region != _region)
            {
                throw new ArgumentException($"Location region must match repository region: {_region}.", nameof(location));
            }
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var location = await _context.Locations
                .Where(l => l.Id == id && l.Region == _region)
                .SingleOrDefaultAsync();
            if (location != null)
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }
        }
    }
}