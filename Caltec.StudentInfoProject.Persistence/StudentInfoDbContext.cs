using Caltec.StudentInfoProject.Domain;
using Microsoft.EntityFrameworkCore;

namespace Caltec.StudentInfoProject.Persistence
{
    public class StudentInfoDbContext : DbContext
    {
        public StudentInfoDbContext()
        {
            
        }
        public StudentInfoDbContext(DbContextOptions<StudentInfoDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<SchoolFees> SchoolFees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Fees)
                .WithOne(f => f.Student)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                  .HasOne(s => s.Class)
                  .WithMany(c => c.Students)
                  .OnDelete(DeleteBehavior.Cascade);

        }
    }
   
}
