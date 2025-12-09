using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Data
{
    // DbContext → IdentityDbContext
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; } = null!;
        public DbSet<Trainer> Trainers { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<TrainerService> TrainerServices { get; set; } = null!;
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity’nin kendi tablolarını oluşturabilmesi için:
            base.OnModelCreating(modelBuilder);

            // TrainerService: Trainer ↔ Service ilişkisi
            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerServices)
                .HasForeignKey(ts => ts.TrainerId);

            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Service)
                .WithMany(s => s.TrainerServices)
                .HasForeignKey(ts => ts.ServiceId);

            modelBuilder.Entity<TrainerService>()
                .HasIndex(ts => new { ts.TrainerId, ts.ServiceId })
                .IsUnique();

            // TrainerAvailability
            modelBuilder.Entity<TrainerAvailability>()
                .HasOne(ta => ta.Trainer)
                .WithMany(t => t.Availabilities)
                .HasForeignKey(ta => ta.TrainerId);

            modelBuilder.Entity<TrainerAvailability>()
                .HasIndex(ta => new { ta.TrainerId, ta.DayOfWeek, ta.StartTime, ta.EndTime })
                .IsUnique();

            // Appointment ilişkileri
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Member)
                .WithMany()
                .HasForeignKey(a => a.MemberId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TrainerId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId);
        }
    }
}
