using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GardenCentresApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GardenCentresApi.Data
{
    public class GardenCentreContext : IdentityDbContext
    {
        public GardenCentreContext(DbContextOptions<GardenCentreContext> options)
            : base(options)
        {
        }

        public DbSet<GardenCentre> GardenCentres { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GardenCentre>()
                .HasOne(gc => gc.Location)
                .WithMany(l => l.GardenCentres)
                .HasForeignKey(gc => gc.LocationId);
        }
    }
}