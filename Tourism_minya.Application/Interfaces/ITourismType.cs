using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Domain.Entities;

namespace Tourism_minya.Application.Interfaces
{
    public interface ITourismType
    {
        Task<IEnumerable<TourismType>> GetAllAsync();
        Task<TourismType?> GetByIdAsync(Guid id);
        Task<TourismType> CreateAsync(TourismType type);
        Task<TourismType> UpdateAsync(Guid id, TourismTypeDTO type);
        Task DeleteAsync(Guid id);
    }
}
