using RefikHaber.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RefikHaber.Utility
{
    public class UygulamaDbContext : IdentityDbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options) { }

        public DbSet<HaberTuru> HaberTurleri { get; set; } 
        public DbSet<Haber> Haberler {  get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
