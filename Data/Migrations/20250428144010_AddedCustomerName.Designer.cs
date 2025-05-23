﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250428144010_AddedCustomerName")]
    partial class AddedCustomerName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("customer_name")
                        .UseCollation("ru-RU-x-icu");

                    b.Property<DateTimeOffset>("OrderDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("order_date");

                    b.Property<short>("PaymentType")
                        .HasColumnType("smallint")
                        .HasColumnName("payment_type");

                    b.Property<short>("Status")
                        .HasColumnType("smallint")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("Domain.OrderProduct", b =>
                {
                    b.Property<int>("OrderId")
                        .HasColumnType("integer")
                        .HasColumnName("order_id");

                    b.Property<string>("ProductName")
                        .HasColumnType("text")
                        .HasColumnName("product_name");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("OrderId", "ProductName")
                        .HasName("pk_order_products");

                    b.HasIndex("ProductName")
                        .HasDatabaseName("ix_order_products_product_name");

                    b.ToTable("order_products", (string)null);
                });

            modelBuilder.Entity("Domain.Product", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name")
                        .UseCollation("ru-RU-x-icu");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.HasKey("Name")
                        .HasName("pk_products");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("Domain.OrderProduct", b =>
                {
                    b.HasOne("Domain.Order", "Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_products_orders_order_id");

                    b.HasOne("Domain.Product", "Product")
                        .WithMany("Orders")
                        .HasForeignKey("ProductName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_products_products_product_name");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Domain.Order", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Domain.Product", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
