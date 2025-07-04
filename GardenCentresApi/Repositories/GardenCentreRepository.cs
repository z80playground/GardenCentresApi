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
    public class GardenCentreRepository : IGardenCentreRepository
    {
        private readonly GardenCentreContext _context;
        private readonly string _region;

        public GardenCentreRepository(GardenCentreContext context, string region)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(region) || (region != "UK" && region != "US"))
            {
                throw new ArgumentException("Region must be 'UK' or 'US'.", nameof(region));
            }
            _region = region;
        }

        public async Task<(List<GardenCentreDto>, int)> GetAllAsync(int page, int pageSize, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            if (page < 1)
            {
                throw new ArgumentException("Page must be greater than 0.", nameof(page));
            }
            if (pageSize < 1)
            {
                throw new ArgumentException("PageSize must be greater than 0.", nameof(pageSize));
            }

            var query = _context.GardenCentres
                .AsNoTracking()
                .Where(gc => gc.Region == _region)
                .Include(gc => gc.Location);

            var totalCount = await query.CountAsync();
            var dtos = await query
                .OrderBy(gc => gc.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(gc => new GardenCentreDto
                {
                    Id = gc.Id,
                    Name = gc.Name,
                    LocationId = gc.LocationId,
                    Region = gc.Region,
                    LocationName = gc.Location.Name
                })
                .ToListAsync();

            return (dtos, totalCount);
        }

        public async Task<GardenCentreDto> GetByIdAsync(int id, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.GardenCentres
                .AsNoTracking()
                .Where(gc => gc.Id == id && gc.Region == _region)
                .Select(gc => new GardenCentreDto
                {
                    Id = gc.Id,
                    Name = gc.Name,
                    LocationId = gc.LocationId,
                    Region = gc.Region,
                    LocationName = gc.Location.Name
                })
                .SingleOrDefaultAsync();
        }

        public async Task<GardenCentreDto> GetByIdWithLocationAsync(int id, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.GardenCentres
                .AsNoTracking()
                .Where(gc => gc.Id == id && gc.Region == _region)
                .Include(gc => gc.Location)
                .Select(gc => new GardenCentreDto
                {
                    Id = gc.Id,
                    Name = gc.Name,
                    LocationId = gc.LocationId,
                    Region = gc.Region,
                    LocationName = gc.Location.Name
                })
                .SingleOrDefaultAsync();
        }

        public async Task AddAsync(GardenCentreDto gardenCentre)
        {
            if (gardenCentre == null)
            {
                throw new ArgumentNullException(nameof(gardenCentre));
            }
            if (gardenCentre.Region != _region)
            {
                throw new ArgumentException($"GardenCentre region must match repository region: {_region}.", nameof(gardenCentre));
            }
            if (!await _context.Locations.AnyAsync(l => l.Id == gardenCentre.LocationId && l.Region == _region))
            {
                throw new ArgumentException($"LocationId {gardenCentre.LocationId} does not exist in region {_region}.", nameof(gardenCentre));
            }
            _context.GardenCentres.Add(new GardenCentre()
            {
                LocationId = gardenCentre.LocationId,
                Name = gardenCentre.Name,
                Region = gardenCentre.Region,
            });
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GardenCentreDto gardenCentre)
        {
            if (gardenCentre == null)
            {
                throw new ArgumentNullException(nameof(gardenCentre));
            }
            if (gardenCentre.Region != _region)
            {
                throw new ArgumentException($"GardenCentre region must match repository region: {_region}.", nameof(gardenCentre));
            }
            if (!await _context.Locations.AnyAsync(l => l.Id == gardenCentre.LocationId && l.Region == _region))
            {
                throw new ArgumentException($"LocationId {gardenCentre.LocationId} does not exist in region {_region}.", nameof(gardenCentre));
            }
            _context.GardenCentres.Update(new GardenCentre()
            {
                Id = gardenCentre.Id,
                LocationId = gardenCentre.LocationId,
                Name = gardenCentre.Name,
                Region = gardenCentre.Region,
            });

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var gardenCentre = await _context.GardenCentres
                .Where(gc => gc.Id == id && gc.Region == _region)
                .SingleOrDefaultAsync();
            if (gardenCentre != null)
            {
                _context.GardenCentres.Remove(gardenCentre);
                await _context.SaveChangesAsync();
            }
        }
    }
}