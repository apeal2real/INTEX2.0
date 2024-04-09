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

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlite("Data Source=INTEX.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CategoryIdPk).HasColumnName("category_ID(PK)");
            entity.Property(e => e.CategoryName).HasColumnName("category_name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.CountryOfResidence).HasColumnName("country_of_residence");
            entity.Property(e => e.CustomerIdPk).HasColumnName("customer_ID(PK)");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.LastName).HasColumnName("last_name");
        });

        modelBuilder.Entity<LineItem>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ProductIdPkFk).HasColumnName("product_ID(PK/FK)");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.TransactionIdPkFk).HasColumnName("transaction_ID(PK/FK)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Bank).HasColumnName("bank");
            entity.Property(e => e.CountryOfTransaction).HasColumnName("country_of_transaction");
            entity.Property(e => e.CustomerIdFk).HasColumnName("customer_ID(FK)");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(e => e.EntryMode).HasColumnName("entry_mode");
            entity.Property(e => e.Fraud).HasColumnName("fraud");
            entity.Property(e => e.ShippingAddress).HasColumnName("shipping_address");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.TransactionIdPk).HasColumnName("transaction_ID(PK)");
            entity.Property(e => e.TypeOfCard).HasColumnName("type_of_card");
            entity.Property(e => e.TypeOfTransaction).HasColumnName("type_of_transaction");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImgLink).HasColumnName("img_link");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.NumParts).HasColumnName("num_parts");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.PrimaryColor).HasColumnName("primary_color");
            entity.Property(e => e.ProductIdPk).HasColumnName("product_ID(PK)");
            entity.Property(e => e.SecondaryColor).HasColumnName("secondary_color");
            entity.Property(e => e.ShortDescription).HasColumnName("short_description");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        // modelBuilder.Entity<ProductsCategory>(entity =>
        // {
        //     entity
        //         .HasNoKey()
        //         .ToTable("Products_Categories");
        //
        //     entity.Property(e => e.CategoryIdPkFk).HasColumnName("category_ID(PK/FK)");
        //     entity.Property(e => e.Field3).HasColumnName("field3");
        //     entity.Property(e => e.Field4).HasColumnName("field4");
        //     entity.Property(e => e.Field5).HasColumnName("field5");
        //     entity.Property(e => e.Field6).HasColumnName("field6");
        //     entity.Property(e => e.Field7).HasColumnName("field7");
        //     entity.Property(e => e.Field8).HasColumnName("field8");
        //     entity.Property(e => e.ProductIdPkFk).HasColumnName("product_ID(PK/FK)");
        // });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
