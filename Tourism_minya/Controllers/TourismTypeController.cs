using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Application.Interfaces;
using Tourism_minya.Domain.Entities;

namespace Tourism_minya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourismTypeController : ControllerBase
    {
        private readonly ITourismType _type;
        private readonly IMapper _mapper;

        public TourismTypeController(ITourismType type, IMapper mapper)
        {
            _type = type;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all Tourism Types.
        /// </summary>
        /// <returns>A list of TourismType records.</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _type.GetAllAsync());

        /// <summary>
        /// Retrieves a specific Tourism Type by its ID.
        /// </summary>
        /// <param name="id">The Tourism Type ID.</param>
        /// <returns>The Tourism Type details.</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpGet("GetbyId/{id}")]
        public async Task<IActionResult> GetById(Guid id) =>
            Ok(await _type.GetByIdAsync(id));

        /// <summary>
        /// Creates a new Tourism Type.
        /// Uses AutoMapper to map the DTO to the domain entity.
        /// </summary>
        /// <param name="dto">The TourismType data transfer object.</param>
        /// <returns>The created Tourism Type and a success message.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPost("PostType")]
        public async Task<IActionResult> Create(TourismTypeDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found in token.");

            var type = _mapper.Map<TourismType>(dto);
            var created = await _type.CreateAsync(type);
            var result = _mapper.Map<TourismTypeDTO>(created);

            return Ok(new { data = result, message = "TourismType Added Successfully" });
        }

        /// <summary>
        /// Updates an existing Tourism Type by ID.
        /// Uses AutoMapper to map the DTO to the domain entity.
        /// </summary>
        /// <param name="id">The ID of the Tourism Type to update.</param>
        /// <param name="typeDTo">The updated TourismType DTO.</param>
        /// <returns>The updated Tourism Type and a success message.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPut("PutTourismType/{id}")]
        public async Task<IActionResult> Update(Guid id, TourismTypeDTO typeDTo)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found in token.");

            var updated = await _type.UpdateAsync(id, typeDTo);
            return Ok(new { data = updated, message = "Type Updated Successfully" });
        }

        /// <summary>
        /// Deletes a Tourism Type by its ID.
        /// </summary>
        /// <param name="id">The Tourism Type ID.</param>
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
                var existing = await _type.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = $"❌ Type with ID {id} not found." });

                await _type.DeleteAsync(id);

                return Ok(new { message = "✅ Type deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "🚨 Cannot delete it", details = ex.Message });
            }
        }
    }
}
