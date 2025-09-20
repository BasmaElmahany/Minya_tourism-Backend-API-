using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Application.Interfaces;
using Tourism_minya.Domain.Entities;

namespace Tourism_minya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CenterController : ControllerBase
    {
        private readonly ICenter _center;
        private readonly IMapper _mapper;

        public CenterController(ICenter center, IMapper mapper)
        {
            _center = center;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all Centers.
        /// </summary>
        /// <returns>A list of Center records.</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _center.GetAllAsync());

        /// <summary>
        /// Retrieves a specific Center by its ID.
        /// </summary>
        /// <param name="id">The Center ID.</param>
        /// <returns>The Center details.</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpGet("GetbyId/{id}")]
        public async Task<IActionResult> GetById(Guid id) =>
            Ok(await _center.GetByIdAsync(id));

        /// <summary>
        /// Creates a new Center.
        /// Uses AutoMapper to map the DTO to the domain entity.
        /// </summary>
        /// <param name="dto">The Center data transfer object.</param>
        /// <returns>The created Center and a success message.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPost("PostType")]
        public async Task<IActionResult> Create(CenterDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found in token.");

            var center = _mapper.Map<Center>(dto);
            var created = await _center.CreateAsync(center);
            var result = _mapper.Map<CenterDto>(created);

            return Ok(new { data = result, message = "Center Added Successfully" });
        }

        /// <summary>
        /// Updates an existing Center by ID.
        /// Uses AutoMapper to map the DTO to the domain entity.
        /// </summary>
        /// <param name="id">The ID of the Center to update.</param>
        /// <param name="DTo">The updated Center DTO.</param>
        /// <returns>The updated Center and a success message.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPut("PutCenter/{id}")]
        public async Task<IActionResult> Update(Guid id, CenterDto DTo)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found in token.");

            var updated = await _center.UpdateAsync(id, DTo);
            return Ok(new { data = updated, message = "Center Updated Successfully" });
        }

        /// <summary>
        /// Deletes a Center by its ID.
        /// </summary>
        /// <param name="id">The Center ID.</param>
        /// <returns>A success message after deletion, or an error if not found.</returns>
        [Authorize(Policy = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found in token.");

            try
            {
                var existing = await _center.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = $"❌ Center with ID {id} not found." });

                await _center.DeleteAsync(id);

                return Ok(new { message = "✅ Center deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "🚨 Cannot delete it", details = ex.Message });
            }
        }
    }
}
