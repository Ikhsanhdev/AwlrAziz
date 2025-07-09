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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}