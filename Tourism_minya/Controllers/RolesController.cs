using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tourism_minya.Application.Interfaces;

namespace Tourism_minya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }
        [Authorize(Policy = "Admin")]
        [HttpPost("{roleName}")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var created = await _roleService.CreateRoleAsync(roleName);
            return created ? Ok("Role created") : BadRequest("Role exists or failed");
        }
    }
}
