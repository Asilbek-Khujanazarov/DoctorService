using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PatientRecoverySystem.DoctorService.Models;

namespace PatientRecoverySystem.DoctorService.Data
{
    public class DoctorDbContext : DbContext
    {
        public DoctorDbContext(DbContextOptions<DoctorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Consultation> Consultations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Schedules)
                .WithOne(s => s.Doctor)
                .HasForeignKey(s => s.DoctorId);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Consultations)
                .WithOne(c => c.Doctor)
                .HasForeignKey(c => c.DoctorId);

            var splitStringConverter = new ValueConverter<List<string>, string>(
         v => v == null ? "" : string.Join(';', v),
         v => string.IsNullOrEmpty(v) ? new List<string>() : v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
     );
            modelBuilder.Entity<Doctor>()
                .Property(d => d.UserIds)
                .HasConversion(splitStringConverter);
        }
    }
}