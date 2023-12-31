﻿// <auto-generated />
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
    [Migration("20231204200747_Initial")]
    partial class Initial
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

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossMarketPrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("AvgPrice")
                        .HasColumnType("float");

                    b.Property<double>("FinalPrice")
                        .HasColumnType("float");

                    b.Property<double>("MeanTrimPrice")
                        .HasColumnType("float");

                    b.Property<int>("MotoCount")
                        .HasColumnType("int");

                    b.Property<int>("MotoMakeId")
                        .HasColumnType("int");

                    b.Property<int>("MotoYearId")
                        .HasColumnType("int");

                    b.Property<double>("StdDevPrice")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("MotoMakeId");

                    b.HasIndex("MotoYearId");

                    b.ToTable("MotocrossMarketPrices");
                });

            modelBuilder.Entity("BaseScraper.Data.Models.MotocrossMarketPrice", b =>
                {
                    b.HasOne("BaseScraper.Data.Models.MotoMake", "Make")
                        .WithMany()
                        .HasForeignKey("MotoMakeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaseScraper.Data.Models.MotoYear", "Year")
                        .WithMany()
                        .HasForeignKey("MotoYearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Make");

                    b.Navigation("Year");
                });
#pragma warning restore 612, 618
        }
    }
}
