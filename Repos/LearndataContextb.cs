using System;
using System.Collections.Generic;
using Dotnet7API.Repos.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet7API.Repos;

public partial class LearndataContextb : DbContext
{
    public LearndataContextb()
    {
    }

    public LearndataContextb(DbContextOptions<LearndataContextb> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblCustomer>(entity =>
        {
            entity.Property(e => e.Code).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
