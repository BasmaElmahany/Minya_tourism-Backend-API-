using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using tourism_minya.Infrastructure.Entities;
using tourism_minya.Infrastructure.Interfaces;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Application.Settings;

namespace Tourism_minya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IUserService _userService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings,
           RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _roleManager = roleManager;
            _userService = userService;
        }

        /// <summary>
        /// Registers a new user with the system.
        /// </summary>
        /// <param name="dto">The user registration data.</param>
        /// <returns>A success message if registration is successful; otherwise, a bad request with an error message.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.RegisterAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Confirms the email address of a user after registration.
        /// </summary>
        /// <param name="userId">The ID of the user to confirm.</param>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>A success message if confirmation is successful; otherwise, a bad request with an error message.</returns>
       /* [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {

            try
            {
                var result = await _userService.ConfirmEmailAsync(userId, token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/


        /// <summary>
        /// Authenticates a user and returns a JWT token on successful login.
        /// </summary>
        /// <param name="dto">The login credentials.</param>
        /// <returns>A JWT token if login is successful; otherwise, an unauthorized response with an error message.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _userService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Invalid Email or password " + "Or you don't have email Register first" });
            }
        }

    }
}
