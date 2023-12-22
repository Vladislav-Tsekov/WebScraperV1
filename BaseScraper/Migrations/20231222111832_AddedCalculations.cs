using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseScraper.Migrations
{
    /// <inheritdoc />
    public partial class AddedCalculations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MedianPrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ModePrice",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceRange",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceVariance",
                table: "MotocrossMarketPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedianPrice",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropColumn(
                name: "ModePrice",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropColumn(
                name: "PriceRange",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropColumn(
                name: "PriceVariance",
                table: "MotocrossMarketPrices");
        }
    }
}
