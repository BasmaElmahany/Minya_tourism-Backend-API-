using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_minya.Infrastructure.Entities;
using Tourism_minya.Domain.Entities;

namespace tourism_minya.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

       public DbSet<TourismType> TourismTypes { get; set; }

        public DbSet<Center> Centers { get; set; }

    }
}
