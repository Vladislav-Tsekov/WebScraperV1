﻿// <auto-generated />
using System;
using BaseScraper.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BaseScraper.Migrations
{
    [DbContext(typeof(MotoContext))]
    [Migration("20240306105843_ExceptionTesting")]
    partial class ExceptionTesting
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BaseScraper.Data.Models.MotoMake", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Make")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Makes");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotoYear", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Years");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossEntry", b =>
                {
                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(450)")
                        .HasComment("Each announcement link is unique, therefore used as a key");

                    b.Property<int>("Cc")
                        .HasColumnType("int")
                        .HasComment("Motorcycle's engine displacement");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2")
                        .HasComment("Date of announcement's addition to the database");

                    b.Property<bool>("IsSold")
                        .HasColumnType("bit")
                        .HasComment("Keeps track whether the motorcycle has been sold");

                    b.Property<int>("MakeId")
                        .HasColumnType("int");

                    b.Property<decimal>("OldPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The price's value before it was changed");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("Motorcycle's actual price");

                    b.Property<int>("PriceChanges")
                        .HasColumnType("int")
                        .HasComment("Announcement's number of price changes");

                    b.Property<int>("YearId")
                        .HasColumnType("int");

                    b.HasKey("Link");

                    b.HasIndex("MakeId");

                    b.HasIndex("YearId");

                    b.ToTable("MotocrossEntries", t =>
                        {
                            t.HasComment("Table of all Motocross announcements");
                        });
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossMarketPrice", b =>
                {
                    b.Property<int>("MakeId")
                        .HasColumnType("int");

                    b.Property<int>("YearId")
                        .HasColumnType("int");

                    b.Property<decimal>("AvgPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The Average Price for each make/year combination");

                    b.Property<decimal>("MeanTrimPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The Mean Price for each make/year combination, calculated with a trim factor of 0.20");

                    b.Property<decimal>("MedianPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The Median Price for each make/year combination");

                    b.Property<decimal>("ModePrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The Mode Price (most frequent value, if such exists) for each make/year combination");

                    b.Property<int>("MotoCount")
                        .HasColumnType("int")
                        .HasComment("The number of motorcycle announcements for the current make/year combination");

                    b.Property<decimal>("PriceRange")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The Price Range (most expensive - cheapest announcement) for each make/year combination");

                    b.Property<decimal>("PriceVariance")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The Price Variance coefficient for each make/year combination");

                    b.Property<decimal>("StdDevPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("The Standard Deviation Price for each make/year combination");

                    b.HasKey("MakeId", "YearId");

                    b.HasIndex("YearId");

                    b.ToTable("MotocrossMarketPrices");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossSoldEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Cc")
                        .HasColumnType("int")
                        .HasComment("Motorcycle's engine displacement");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2")
                        .HasComment("Date of announcement's addition to the database");

                    b.Property<DateTime>("DateSold")
                        .HasColumnType("datetime2")
                        .HasComment("Date of announcement's removal from the website");

                    b.Property<int>("MakeId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("Motorcycle's price");

                    b.Property<int>("YearId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MakeId");

                    b.HasIndex("YearId");

                    b.ToTable("MotocrossSoldEntries", t =>
                        {
                            t.HasComment("A table of all sold motorcycles");
                        });
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossEntry", b =>
                {
                    b.HasOne("BaseScraper.Data.Models.MotoMake", "Make")
                        .WithMany("MotocrossEntries")
                        .HasForeignKey("MakeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaseScraper.Data.Models.MotoYear", "Year")
                        .WithMany("MotocrossEntries")
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Make");

                    b.Navigation("Year");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossMarketPrice", b =>
                {
                    b.HasOne("BaseScraper.Data.Models.MotoMake", "Make")
                        .WithMany("MotocrossMarketPrices")
                        .HasForeignKey("MakeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaseScraper.Data.Models.MotoYear", "Year")
                        .WithMany("MotocrossMarketPrices")
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Make");

                    b.Navigation("Year");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossSoldEntry", b =>
                {
                    b.HasOne("BaseScraper.Data.Models.MotoMake", "Make")
                        .WithMany("MotocrossSoldEntries")
                        .HasForeignKey("MakeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaseScraper.Data.Models.MotoYear", "Year")
                        .WithMany("MotocrossSoldEntries")
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Make");

                    b.Navigation("Year");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotoMake", b =>
                {
                    b.Navigation("MotocrossEntries");

                    b.Navigation("MotocrossMarketPrices");

                    b.Navigation("MotocrossSoldEntries");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotoYear", b =>
                {
                    b.Navigation("MotocrossEntries");

                    b.Navigation("MotocrossMarketPrices");

                    b.Navigation("MotocrossSoldEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
