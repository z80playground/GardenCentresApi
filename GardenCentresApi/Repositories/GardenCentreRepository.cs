using GardenCentresApi.Data;
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

        public async Task<(List<GardenCentre>, int)> GetAllAsync(int page, int pageSize, string region)
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
                .Where(gc => gc.Region == _region);

            var totalCount = await query.CountAsync();
            var gardenCentres = await query
                .OrderBy(gc => gc.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (gardenCentres, totalCount);
        }

        public async Task<GardenCentre> GetByIdAsync(int id, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.GardenCentres
                .AsNoTracking()
                .Where(gc => gc.Id == id && gc.Region == _region)
                .SingleOrDefaultAsync();
        }

        public async Task<GardenCentre> GetByIdWithLocationAsync(int id, string region)
        {
            if (region != _region)
            {
                throw new ArgumentException($"Region must match repository region: {_region}.", nameof(region));
            }
            return await _context.GardenCentres
                .AsNoTracking()
                .Where(gc => gc.Id == id && gc.Region == _region)
                .Include(gc => gc.Location)
                .SingleOrDefaultAsync();
        }

        public async Task AddAsync(GardenCentre gardenCentre)
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
            _context.GardenCentres.Add(gardenCentre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GardenCentre gardenCentre)
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
            _context.GardenCentres.Update(gardenCentre);
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