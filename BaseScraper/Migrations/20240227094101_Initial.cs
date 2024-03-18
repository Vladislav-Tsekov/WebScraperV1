using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseScraper.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Makes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Make = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Makes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Years",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Years", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MotocrossEntries",
                columns: table => new
                {
                    Link = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Each announcement link is unique, therefore used as a key"),
                    MakeId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    Cc = table.Column<int>(type: "int", nullable: false, comment: "Motorcycle's engine displacement"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "Motorcycle's actual price"),
                    PriceChanges = table.Column<int>(type: "int", nullable: false, comment: "Announcement's number of price changes"),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The price's value before it was changed"),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date of announcement's addition to the database"),
                    IsSold = table.Column<bool>(type: "bit", nullable: false, comment: "Keeps track whether the motorcycle has been sold")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotocrossEntries", x => x.Link);
                    table.ForeignKey(
                        name: "FK_MotocrossEntries_Makes_MakeId",
                        column: x => x.MakeId,
                        principalTable: "Makes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotocrossEntries_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Table of all Motocross announcements");

            migrationBuilder.CreateTable(
                name: "MotocrossMarketPrices",
                columns: table => new
                {
                    MakeId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    MotoCount = table.Column<int>(type: "int", nullable: false, comment: "The number of motorcycle announcements for the current make/year combination"),
                    AvgPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The Average Price for each make/year combination"),
                    MeanTrimPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The Mean Price for each make/year combination, calculated with a trim factor of 0.20"),
                    StdDevPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The Standard Deviation Price for each make/year combination"),
                    MedianPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The Median Price for each make/year combination"),
                    ModePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The Mode Price (most frequent value, if such exists) for each make/year combination"),
                    PriceRange = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The Price Range (most expensive - cheapest announcement) for each make/year combination"),
                    PriceVariance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The Price Variance coefficient for each make/year combination")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotocrossMarketPrices", x => new { x.MakeId, x.YearId });
                    table.ForeignKey(
                        name: "FK_MotocrossMarketPrices_Makes_MakeId",
                        column: x => x.MakeId,
                        principalTable: "Makes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotocrossMarketPrices_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MotocrossSoldEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MakeId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    Cc = table.Column<int>(type: "int", nullable: false, comment: "Motorcycle's engine displacement"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "Motorcycle's price"),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date of announcement's addition to the database"),
                    DateSold = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date of announcement's removal from the website")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotocrossSoldEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotocrossSoldEntries_Makes_MakeId",
                        column: x => x.MakeId,
                        principalTable: "Makes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotocrossSoldEntries_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "A table of all sold motorcycles");

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossEntries_MakeId",
                table: "MotocrossEntries",
                column: "MakeId");

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossEntries_YearId",
                table: "MotocrossEntries",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossMarketPrices_YearId",
                table: "MotocrossMarketPrices",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossSoldEntries_MakeId",
                table: "MotocrossSoldEntries",
                column: "MakeId");

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossSoldEntries_YearId",
                table: "MotocrossSoldEntries",
                column: "YearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MotocrossEntries");

            migrationBuilder.DropTable(
                name: "MotocrossMarketPrices");

            migrationBuilder.DropTable(
                name: "MotocrossSoldEntries");

            migrationBuilder.DropTable(
                name: "Makes");

            migrationBuilder.DropTable(
                name: "Years");
        }
    }
}
