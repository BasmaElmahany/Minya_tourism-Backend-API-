using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tourism_minya.Application.DTOs;

namespace tourism_minya.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterUserDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
