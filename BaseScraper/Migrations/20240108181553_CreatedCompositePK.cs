using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseScraper.Migrations
{
    /// <inheritdoc />
    public partial class CreatedCompositePK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MotocrossMarketPrices",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropIndex(
                name: "IX_MotocrossMarketPrices_MakeId",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MotocrossMarketPrices");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MotocrossMarketPrices",
                table: "MotocrossMarketPrices",
                columns: new[] { "MakeId", "YearId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MotocrossMarketPrices",
                table: "MotocrossMarketPrices");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MotocrossMarketPrices",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MotocrossMarketPrices",
                table: "MotocrossMarketPrices",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossMarketPrices_MakeId",
                table: "MotocrossMarketPrices",
                column: "MakeId");
        }
    }
}
