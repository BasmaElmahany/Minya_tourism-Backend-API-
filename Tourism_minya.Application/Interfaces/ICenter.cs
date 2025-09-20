using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Domain.Entities;

namespace Tourism_minya.Application.Interfaces
{
    public interface ICenter
    {
        Task<IEnumerable<Center>> GetAllAsync();
        Task<Center?> GetByIdAsync(Guid id);
        Task<Center> CreateAsync(Center center);
        Task<Center> UpdateAsync(Guid id, CenterDto center);
        Task DeleteAsync(Guid id);
    }
}
