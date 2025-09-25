using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tourism_minya.Infrastructure.Entities;
using tourism_minya.Infrastructure.Interfaces;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Application.Settings;

namespace tourism_minya.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
           
        }

        public async Task<string> RegisterAsync(RegisterUserDto dto)
        {
            var user = _mapper.Map<ApplicationUser>(dto);
            user.UserName = dto.Email;

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Auto-confirm
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                throw new Exception(string.Join(", ", confirmResult.Errors.Select(e => e.Description)));

            // You can still return a success message or directly issue a JWT
            return "User registered successfully.";
        }


        /*
                public async Task<string> ConfirmEmailAsync(string userId, string token)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                        throw new Exception("Invalid user ID");

                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (!result.Succeeded)
                        throw new Exception("Email confirmation failed");

                    // Assign role if not already assigned
                    if (!await _userManager.IsInRoleAsync(user, user.DesiredRole))
                    {
                        var roleExists = await _userManager.AddToRoleAsync(user, user.DesiredRole);
                        if (!roleExists.Succeeded)
                        {
                            throw new Exception("Failed to assign role.");
                        }
                    }

                    return "Email confirmed and role assigned.";
                }*/


        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new Exception("Invalid credentials");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new Exception("Please confirm your email first.");

            return await GenerateJwtToken(user);
        }

        private async Task<AuthResponseDto> GenerateJwtToken(ApplicationUser user)
        {
            var claims = await GetUserClaimsAsync(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = tokenString,
                IsAuthenticated = true,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName ?? user.UserName,
                Roles = roles.ToList()
            };
        }


        private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
                {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.FullName ?? user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));


            var isMember = await _userManager.IsInRoleAsync(user, "Member");
            if (isMember)
                claims.Add(new Claim(ClaimTypes.Role, "Member"));
            return claims;
        }


        public async Task<string> AssignRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // Ensure the role exists
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                    throw new Exception("Failed to assign role: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                return $"Role '{roleName}' assigned to user {user.Email}.";
            }

            return $"User {user.Email} already has role '{roleName}'.";
        }



        public async Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            // Extract userId from claims
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User ID not found in token claims.");

            // Fetch user from Identity
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Map user to DTO
            var userDto = _mapper.Map<CurrentUserDto>(user);
            userDto.Roles = roles;

            return userDto;
        }


        public async Task<List<CurrentUserDto>> GetAllUsersAsync()
        {
            // Fetch all users from the identity store
            var users = _userManager.Users.ToList();

            var userDtos = new List<CurrentUserDto>();

            foreach (var user in users)
            {
                var dto = _mapper.Map<CurrentUserDto>(user);
                dto.Roles = await _userManager.GetRolesAsync(user); // include roles
                userDtos.Add(dto);
            }

            return userDtos;
        }


        public async Task<string> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return "Password changed successfully.";
        }

        public async Task<string> GenerateResetPasswordTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            // You’d normally send this via email
            var resetLink = $"http://localhost:12957/api/Auth/resetpassword?email={user.Email}&token={encodedToken}";

            return resetLink;
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("User not found");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return "Password reset successfully.";
        }



    }
}
