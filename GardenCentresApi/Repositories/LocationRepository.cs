using GardenCentresApi.Data;
using GardenCentresApi.Dto;
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

        public async Task<List<LocationDto>> GetAllAsync(string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.Locations
                .AsNoTracking()
                .Where(l => l.Region == _region)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Region = l.Region
                })
                .ToListAsync();
        }

        public async Task<LocationDto> GetByIdAsync(int id, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.Locations
                .AsNoTracking()
                .Where(l => l.Id == id && l.Region == _region)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Region = l.Region
                })
                .SingleOrDefaultAsync();
        }

        public async Task<List<GardenCentreDto>> GetGardenCentresByLocationAsync(int locationId, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.GardenCentres
                .AsNoTracking()
                .Where(gc => gc.LocationId == locationId && gc.Region == _region)
                .Include(gc => gc.Location)
                .Select(gc => new GardenCentreDto
                {
                    Id = gc.Id,
                    Name = gc.Name,
                    LocationId = gc.LocationId,
                    Region = gc.Region,
                    LocationName = gc.Location.Name
                })
                .ToListAsync();
        }

        public async Task<LocationDto> AddAsync(CreateLocationDto locationDto)
        {
            if (locationDto == null)
            {
                throw new ArgumentNullException(nameof(locationDto));
            }
            if (locationDto.Region != _region)
            {
                throw new ArgumentException($"Location region must match repository region: {_region}.", nameof(locationDto));
            }
            var location = new Location()
            {
                Name = locationDto.Name,
                Region = locationDto.Region
            };
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return new LocationDto()
            {
                Id = location.Id,
                Name = location.Name,
                Region = location.Region
            };
        }

        public async Task UpdateAsync(int id, UpdateLocationDto location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }
            if (location.Region != _region)
            {
                throw new ArgumentException($"Location region must match repository region: {_region}.", nameof(location));
            }
            _context.Locations.Update(new Location()
            {
                Id = id,
                Name = location.Name,
                Region = location.Region
            });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var location = await _context.Locations
                .Where(l => l.Id == id && l.Region == _region)
                .SingleOrDefaultAsync();
            if (location == null)
            {
                return;
            }
            if (await _context.GardenCentres.AnyAsync(gc => gc.LocationId == id && gc.Region == _region))
            {
                throw new InvalidOperationException($"Cannot delete Location {id} as it has associated GardenCentres.");
            }
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }
    }
}