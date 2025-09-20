using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tourism_minya.Infrastructure.Persistence;
using Tourism_minya.Application.DTOs;
using Tourism_minya.Application.Interfaces;
using Tourism_minya.Domain.Entities;

namespace tourism_minya.Infrastructure.Services
{
    public class CenterService : ICenter
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CenterService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Center>> GetAllAsync()
        {
            var Centers = await _context.Centers.AsNoTracking().ToListAsync();
            return Centers;
        }

        public async Task<Center?> GetByIdAsync(Guid id)
        {
            if (id == null)
                throw new Exception("Center not found"); ;

            var Center = await _context.Centers

            .FirstOrDefaultAsync(c => c.Id == id);
            return Center;

        }


        public async Task<Center> CreateAsync(Center center)
        {
            _context.Centers.Add(center);
            await _context.SaveChangesAsync();
            return center;
        }

        public async Task<Center> UpdateAsync(Guid id, CenterDto Dto)
        {
            var existingCenter = await _context.Centers.FindAsync(id);

            if (existingCenter == null) throw new Exception("Center not found");



            _mapper.Map(Dto, existingCenter);

            _context.Entry(existingCenter).Property(c => c.Name).IsModified = true;


            await _context.SaveChangesAsync();

            return existingCenter;


        }

        public async Task DeleteAsync(Guid id)
        {
            var center = await _context.Centers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (center == null)
                throw new Exception("center not found");

            _context.Centers.Remove(center);
            await _context.SaveChangesAsync();
        }

    }
}

