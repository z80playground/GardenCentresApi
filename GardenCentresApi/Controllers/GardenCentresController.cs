using Asp.Versioning;
using GardenCentresApi.Models;
using GardenCentresApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace GardenCentresApi.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GardenCentresController : ControllerBase
    {
        private readonly IGardenCentreRepository _gardenCentreRepository;
        private readonly int _defaultPageSize;

        public GardenCentresController(IGardenCentreRepository gardenCentreRepository, IConfiguration configuration)
        {
            _gardenCentreRepository = gardenCentreRepository ?? throw new ArgumentNullException(nameof(gardenCentreRepository));
            _defaultPageSize = configuration.GetValue<int>("Pagination:DefaultPageSize", 10);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 0)
        {
            if (page < 1)
            {
                return BadRequest("Page must be greater than 0.");
            }
            if (pageSize <= 0)
            {
                pageSize = _defaultPageSize;
            }

            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var (gardenCentres, totalCount) = await _gardenCentreRepository.GetAllAsync(page, pageSize, region);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Data = gardenCentres
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var gardenCentre = await _gardenCentreRepository.GetByIdWithLocationAsync(id, region);
            if (gardenCentre == null)
            {
                return NotFound();
            }

            return Ok(gardenCentre);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GardenCentre gardenCentre)
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

            gardenCentre.Region = region; // Ensure Region matches JWT claim
            try
            {
                await _gardenCentreRepository.AddAsync(gardenCentre);
                return CreatedAtAction(nameof(GetById), new { id = gardenCentre.Id }, gardenCentre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GardenCentre updatedCentre)
        {
            if (!ModelState.IsValid || id != updatedCentre.Id)
            {
                return BadRequest(ModelState);
            }

            var region = User.FindFirst("Region")?.Value;
            if (string.IsNullOrWhiteSpace(region))
            {
                return Unauthorized("Region claim is missing.");
            }

            var existing = await _gardenCentreRepository.GetByIdAsync(id, region);
            if (existing == null)
            {
                return NotFound();
            }

            updatedCentre.Region = region; // Ensure Region matches JWT claim
            try
            {
                await _gardenCentreRepository.UpdateAsync(updatedCentre);
                return NoContent();
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

            var existing = await _gardenCentreRepository.GetByIdAsync(id, region);
            if (existing == null)
            {
                return NotFound();
            }

            await _gardenCentreRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}