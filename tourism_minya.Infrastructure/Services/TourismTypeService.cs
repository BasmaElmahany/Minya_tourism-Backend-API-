using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_minya.Infrastructure.Persistence;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Application.Interfaces;
using Tourism_minya.Domain.Entities;

namespace tourism_minya.Infrastructure.Services
{
    public class TourismTypeService : ITourismType
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public TourismTypeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;

            _mapper = mapper;

        }
        public async Task<IEnumerable<TourismType>> GetAllAsync()
        {
            var TourismTypes = await _context.TourismTypes.AsNoTracking().ToListAsync();
            return TourismTypes;
        }

        public async Task<TourismType?> GetByIdAsync(Guid id)
        {
            if (id == null)
                throw new Exception("Type not found"); ;

            var catalog = await _context.TourismTypes
           
            .FirstOrDefaultAsync(c => c.Id == id);
            return catalog;

        }


        public async Task<TourismType> CreateAsync(TourismType type)
        {
            _context.TourismTypes.Add(type);
            await _context.SaveChangesAsync();
            return type;
        }

        public async Task<TourismType> UpdateAsync(Guid id, TourismTypeDTO typeDto)
        {
            var existingType = await _context.TourismTypes.FindAsync(id);

            if (existingType == null) throw new Exception("TourismType not found");



            _mapper.Map(typeDto, existingType);

            _context.Entry(existingType).Property(c => c.Name).IsModified = true;
            

            await _context.SaveChangesAsync();

            return existingType;


        }

        public async Task DeleteAsync(Guid id)
        {
            var type = await _context.TourismTypes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (type == null)
                throw new Exception("type not found");

            _context.TourismTypes.Remove(type);
            await _context.SaveChangesAsync();
        }

    }
}
