using Asp.Versioning;
using GardenCentresApi.Dto;
using GardenCentresApi.Models;
using GardenCentresApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GardenCentresApi.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;

        public LocationsController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var locations = await _locationRepository.GetAllAsync(region);
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var location = await _locationRepository.GetByIdAsync(id, region);
            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        [HttpGet("{id}/GardenCentres")]
        public async Task<IActionResult> GetGardenCentresByLocation(int id)
        {
            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var gardenCentres = await _locationRepository.GetGardenCentresByLocationAsync(id, region);
            return Ok(gardenCentres);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationDto location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            location.Region = region; // Ensure Region matches JWT claim
            try
            {
                var newLocation = await _locationRepository.AddAsync(location);
                return CreatedAtAction(nameof(GetById), new { id = newLocation.Id }, newLocation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLocationDto updatedLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var existing = await _locationRepository.GetByIdAsync(id, region);
            if (existing == null)
            {
                return NotFound();
            }

            updatedLocation.Region = region; // Ensure Region matches JWT claim
            try
            {
                await _locationRepository.UpdateAsync(id, updatedLocation);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var existing = await _locationRepository.GetByIdAsync(id, region);
            if (existing == null)
            {
                return NotFound();
            }

            try
            {
                await _locationRepository.DeleteAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // E.g., Location has associated GardenCentres
            }
        }
    }
}