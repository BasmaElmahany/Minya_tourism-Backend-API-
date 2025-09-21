using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tourism_minya.Application.DTOs;

namespace tourism_minya.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterUserDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<string> AssignRoleAsync(string userId, string roleName);
        Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal principal);
        Task<List<CurrentUserDto>> GetAllUsersAsync();

        Task<string> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<string> GenerateResetPasswordTokenAsync(string email);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
