using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace INTEX2._0.Models;

public partial class IntexContext : DbContext
{
    public IntexContext()
    {
    }

    public IntexContext(DbContextOptions<IntexContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<LineItem> LineItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductsCategory> ProductsCategories { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlite("Data Source=INTEX.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Field1).HasColumnName("field1");
            entity.Property(e => e.Field2).HasColumnName("field2");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Field1).HasColumnName("field1");
            entity.Property(e => e.Field2).HasColumnName("field2");
            entity.Property(e => e.Field3).HasColumnName("field3");
            entity.Property(e => e.Field4).HasColumnName("field4");
            entity.Property(e => e.Field5).HasColumnName("field5");
            entity.Property(e => e.Field6).HasColumnName("field6");
            entity.Property(e => e.Field7).HasColumnName("field7");
        });

        modelBuilder.Entity<LineItem>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Field1).HasColumnName("field1");
            entity.Property(e => e.Field2).HasColumnName("field2");
            entity.Property(e => e.Field3).HasColumnName("field3");
            entity.Property(e => e.Field4).HasColumnName("field4");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Field1).HasColumnName("field1");
            entity.Property(e => e.Field10).HasColumnName("field10");
            entity.Property(e => e.Field11).HasColumnName("field11");
            entity.Property(e => e.Field12).HasColumnName("field12");
            entity.Property(e => e.Field13).HasColumnName("field13");
            entity.Property(e => e.Field2).HasColumnName("field2");
            entity.Property(e => e.Field3).HasColumnName("field3");
            entity.Property(e => e.Field4).HasColumnName("field4");
            entity.Property(e => e.Field5).HasColumnName("field5");
            entity.Property(e => e.Field6).HasColumnName("field6");
            entity.Property(e => e.Field7).HasColumnName("field7");
            entity.Property(e => e.Field8).HasColumnName("field8");
            entity.Property(e => e.Field9).HasColumnName("field9");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Field1).HasColumnName("field1");
            entity.Property(e => e.Field10).HasColumnName("field10");
            entity.Property(e => e.Field2).HasColumnName("field2");
            entity.Property(e => e.Field3).HasColumnName("field3");
            entity.Property(e => e.Field4).HasColumnName("field4");
            entity.Property(e => e.Field5).HasColumnName("field5");
            entity.Property(e => e.Field6).HasColumnName("field6");
            entity.Property(e => e.Field7).HasColumnName("field7");
            entity.Property(e => e.Field8).HasColumnName("field8");
            entity.Property(e => e.Field9).HasColumnName("field9");
        });

        modelBuilder.Entity<ProductsCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Products_Categories");

            entity.Property(e => e.Field1).HasColumnName("field1");
            entity.Property(e => e.Field2).HasColumnName("field2");
            entity.Property(e => e.Field3).HasColumnName("field3");
            entity.Property(e => e.Field4).HasColumnName("field4");
            entity.Property(e => e.Field5).HasColumnName("field5");
            entity.Property(e => e.Field6).HasColumnName("field6");
            entity.Property(e => e.Field7).HasColumnName("field7");
            entity.Property(e => e.Field8).HasColumnName("field8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
