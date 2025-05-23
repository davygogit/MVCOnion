using Microsoft.EntityFrameworkCore;
using Domain;

namespace Infrastructure
{
    public  class MVCOnionContext : DbContext
    {
        public MVCOnionContext(DbContextOptions<MVCOnionContext> options) : base(options)
        {
        }
        public DbSet<Etage> Etages { get; set; }
        public DbSet<Bureau> Bureaux { get; set; }
    }
}
