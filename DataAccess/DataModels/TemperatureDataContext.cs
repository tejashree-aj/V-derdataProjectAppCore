using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DataAccess.DataModels
{
    public partial class TemperatureDataContext : DbContext
    {
        public TemperatureDataContext()
        {
            Database.SetCommandTimeout(150000);
        }

        public TemperatureDataContext(DbContextOptions<TemperatureDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TemperatureData> Temperatures { get; set; }
        public DbSet<TemperatureChange> TemperatureChanges { get; set; }

        public DbSet<AverageTemperatureView> AutumnAndWinterTemperatureDate { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TemperatureData>(entity =>
            {
                entity.HasKey(e => e.Temperature_Id);

                entity.ToTable("Temperature");

                entity.Property(e => e.Place)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TemperatureChange>(entity =>
            {
                entity.HasNoKey();
            });

            OnModelCreatingPartial(modelBuilder);

            modelBuilder.Entity<AverageTemperatureView>(entity =>
            {
                entity.HasNoKey();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
