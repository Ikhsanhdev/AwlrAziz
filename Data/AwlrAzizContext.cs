using System;
using System.Collections.Generic;
using AwlrAziz.Models;
using Microsoft.EntityFrameworkCore;

namespace AwlrAziz.Data;

public partial class AwlrAzizContext : DbContext {
    public AwlrAzizContext() {}

    public AwlrAzizContext(DbContextOptions<AwlrAzizContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}