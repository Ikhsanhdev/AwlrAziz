using System;
using System.Collections.Generic;
using AwlrAziz.Models;
using Microsoft.EntityFrameworkCore;

namespace AwlrAziz.Data;

public partial class AwlrAzizContext : DbContext {
    public AwlrAzizContext() {}

    public AwlrAzizContext(DbContextOptions<AwlrAzizContext> options) : base(options) {}

    public virtual DbSet<MvDevice> MvDevices { get; set; }
    public virtual DbSet<Device> Devices { get; set; }
    public virtual DbSet<Station> Stations { get; set; }
    public virtual DbSet<AwlrLastReading> AwlrLastReadings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => new { e.StationId, e.DeviceId }).HasName("devices_pkey");

            // entity.HasIndex(e => e.BrandCode, "idx_devices_brand_code");

            // entity.HasIndex(e => e.DeviceId, "idx_devices_device_id");

            // entity.HasIndex(e => e.StationId, "idx_devices_station_id");

            entity.Property(e => e.DeviceId).HasMaxLength(255);
            entity.Property(e => e.BrandCode).HasMaxLength(4);
            entity.Property(e => e.NoGsm)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");

            // entity.HasOne(d => d.Station).WithMany(p => p.Devices)
            //     .HasForeignKey(d => d.StationId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("fk_devices_station_id");
        });

        modelBuilder.Entity<MvDevice>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("MvDevices");

            entity.Property(e => e.BrandCode).HasMaxLength(4);
            entity.Property(e => e.BrandJobName).HasMaxLength(255);
            entity.Property(e => e.BrandName).HasMaxLength(255);
            entity.Property(e => e.BrandPassword).HasMaxLength(255);
            entity.Property(e => e.BrandUrl).HasMaxLength(255);
            entity.Property(e => e.BrandUsername).HasMaxLength(255);
            entity.Property(e => e.DeletedAt).HasColumnType("timestamp(0) without time zone");
            // entity.Property(e => e.InstalledDate).HasColumnType("date");
            entity.Property(e => e.DeviceId).HasMaxLength(255);
            entity.Property(e => e.NoGsm).HasMaxLength(255);
            entity.Property(e => e.OrganizationCode).HasMaxLength(10);
            entity.Property(e => e.OrganizationName).HasMaxLength(255);
            entity.Property(e => e.StationName).HasMaxLength(255);
            entity.Property(e => e.StationType).HasMaxLength(255);
            entity.Property(e => e.SubDomain).HasMaxLength(150);
            entity.Property(e => e.SubDomainOld).HasMaxLength(150);
            entity.Property(e => e.TimeZone).HasMaxLength(4);
            entity.Property(e => e.UnitDebit).HasMaxLength(255);
            entity.Property(e => e.UnitDisplay).HasMaxLength(255);
            entity.Property(e => e.UnitSensor).HasMaxLength(255);
        });

        modelBuilder.Entity<AwlrLastReading>(entity =>
        {
            entity.ToTable("AwlrLastReadings");

            // entity.HasKey(e => new { e.StationId, e.DeviceId }).HasName("awlr_last_readings_pkey");
            entity.HasKey(e => e.Id).HasName("awlr_last_readings_pkey");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");
            entity.Property(e => e.StationId)
                .ValueGeneratedNever()
                .HasColumnName("StationId");
            entity.Property(e => e.DeviceId)
                .ValueGeneratedNever()
                .HasColumnName("DeviceId");
            entity.Property(e => e.ReadingAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("ReadingAt");
            entity.Property(e => e.ChangeStatus)
                .HasMaxLength(255)
                .HasColumnName("ChangeStatus");
            entity.Property(e => e.WarningStatus)
                .HasMaxLength(255)
                .HasColumnName("WarningStatus");
            entity.Property(e => e.WaterLevel).HasColumnName("WaterLevel");
            entity.Property(e => e.ChangeValue).HasColumnName("ChangeValue");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Stations_pkey");
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp(0) without time zone");
            entity.Property(e => e.DeletedAt).HasColumnType("timestamp(0) without time zone");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp(0) without time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}