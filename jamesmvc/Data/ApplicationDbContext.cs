using jamesmvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static jamesmvc.Models.LogisticsOrder;
namespace jamesmvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<LogisticsOrder> LogisticsOrder { get; set; }
        public DbSet<DriverProfile> DriverProfiles { get; set; }
        public DbSet<TrackingRecord> TrackingRecords { get; set; }

        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
    }
}
