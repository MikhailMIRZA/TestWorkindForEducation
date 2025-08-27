using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Class).IsRequired().HasMaxLength(50);
                entity.Property(r => r.Price).HasColumnType("decimal(18,2)");
                entity.Property(r => r.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.UserId).IsRequired();
                entity.Property(b => b.UserName).IsRequired().HasMaxLength(100);
                entity.Property(b => b.UserEmail).IsRequired().HasMaxLength(100);

                entity.HasOne(b => b.Room)
                      .WithMany(r => r.Bookings)
                      .HasForeignKey(b => b.RoomId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(b => new { b.RoomId, b.StartDate, b.EndDate });
            });
        }
    }
}

