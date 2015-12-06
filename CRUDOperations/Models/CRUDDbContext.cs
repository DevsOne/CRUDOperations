using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CRUDOperations.Models
{
    public class CRUDDbContext : DbContext
    {
        public CRUDDbContext() : base("CRUDConnection")
        {
        }
        public static CRUDDbContext Create()
        {
            return new CRUDDbContext();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<UserCountry> UserCountries { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<UserDescription> UserDescriptions { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
