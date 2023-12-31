﻿// <auto-generated />
using System;
using BaseScraper.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BaseScraper.Migrations
{
    [DbContext(typeof(MotoContext))]
    partial class MotoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Cc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateSold")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsSold")
                        .HasColumnType("bit");

                    b.Property<int>("MakeId")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("YearId")
                        .HasColumnType("int");

                    b.HasKey("Link");

                    b.HasIndex("MakeId");

                    b.HasIndex("YearId");

                    b.ToTable("MotocrossEntries");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossMarketPrice", b =>
                {
                    b.Property<int>("MakeId")
                        .HasColumnType("int");

                    b.Property<int>("YearId")
                        .HasColumnType("int");

                    b.Property<decimal>("AvgPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("FinalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MeanTrimPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MedianPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ModePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MotoCount")
                        .HasColumnType("int");

                    b.Property<decimal>("PriceRange")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceVariance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("StdDevPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("MakeId", "YearId");

                    b.HasIndex("YearId");

                    b.ToTable("MotocrossMarketPrices");
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

            modelBuilder.Entity("BaseScraper.Data.Models.MotoMake", b =>
                {
                    b.Navigation("MotocrossEntries");

                    b.Navigation("MotocrossMarketPrices");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotoYear", b =>
                {
                    b.Navigation("MotocrossEntries");

                    b.Navigation("MotocrossMarketPrices");
                });
#pragma warning restore 612, 618
        }
    }
}
