using Microsoft.EntityFrameworkCore;
using Task5.Models;

namespace Task5.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<FirstName> FirstNames { get; set; }
        public DbSet<LastName> LastNames { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):
            base(options)
        {}

        public ApplicationDbContext():base(){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=database-1.cwwsbjhfixu7.eu-central-1.rds.amazonaws.com,1433;Database=task5;Integrated Security=false;User ID=admin;Password=FuLtaWdROU;");
        }
    }
}
