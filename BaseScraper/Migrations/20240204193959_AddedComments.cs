using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseScraper.Migrations
{
    /// <inheritdoc />
    public partial class AddedComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "MotocrossSoldEntries",
                comment: "A table of all sold motorcycles");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "MotocrossSoldEntries",
                type: "float",
                nullable: false,
                comment: "Motorcycle's price",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSold",
                table: "MotocrossSoldEntries",
                type: "datetime2",
                nullable: false,
                comment: "Date of announcement's removal from the website",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                table: "MotocrossSoldEntries",
                type: "datetime2",
                nullable: false,
                comment: "Date of announcement's addition to the database",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Cc",
                table: "MotocrossSoldEntries",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Motorcycle's engine displacement",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "StdDevPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The Standard Deviation Price for each make/year combination",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceVariance",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The Price Variance coefficient for each make/year combination",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceRange",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The Price Range (most expensive - cheapest announcement) for each make/year combination",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "MotoCount",
                table: "MotocrossMarketPrices",
                type: "int",
                nullable: false,
                comment: "The number of motorcycle announcements for the current make/year combination",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "ModePrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The Mode Price (most frequent value, if such exists) for each make/year combination",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MedianPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The Median Price for each make/year combination",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MeanTrimPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The Mean Price for each make/year combination, calculated with a trim factor of 0.20",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AvgPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The Average Price for each make/year combination",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "MotocrossSoldEntries",
                oldComment: "A table of all sold motorcycles");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "MotocrossSoldEntries",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Motorcycle's price");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSold",
                table: "MotocrossSoldEntries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date of announcement's removal from the website");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                table: "MotocrossSoldEntries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date of announcement's addition to the database");

            migrationBuilder.AlterColumn<string>(
                name: "Cc",
                table: "MotocrossSoldEntries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Motorcycle's engine displacement");

            migrationBuilder.AlterColumn<decimal>(
                name: "StdDevPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The Standard Deviation Price for each make/year combination");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceVariance",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The Price Variance coefficient for each make/year combination");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceRange",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The Price Range (most expensive - cheapest announcement) for each make/year combination");

            migrationBuilder.AlterColumn<int>(
                name: "MotoCount",
                table: "MotocrossMarketPrices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The number of motorcycle announcements for the current make/year combination");

            migrationBuilder.AlterColumn<decimal>(
                name: "ModePrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The Mode Price (most frequent value, if such exists) for each make/year combination");

            migrationBuilder.AlterColumn<decimal>(
                name: "MedianPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The Median Price for each make/year combination");

            migrationBuilder.AlterColumn<decimal>(
                name: "MeanTrimPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The Mean Price for each make/year combination, calculated with a trim factor of 0.20");

            migrationBuilder.AlterColumn<decimal>(
                name: "AvgPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The Average Price for each make/year combination");
        }
    }
}
