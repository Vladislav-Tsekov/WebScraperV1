using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseScraper.Migrations
{
    /// <inheritdoc />
    public partial class EntriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotocrossMarketPrices_Makes_MotoMakeId",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_MotocrossMarketPrices_Years_MotoYearId",
                table: "MotocrossMarketPrices");

            migrationBuilder.RenameColumn(
                name: "MotoYearId",
                table: "MotocrossMarketPrices",
                newName: "YearId");

            migrationBuilder.RenameColumn(
                name: "MotoMakeId",
                table: "MotocrossMarketPrices",
                newName: "MakeId");

            migrationBuilder.RenameIndex(
                name: "IX_MotocrossMarketPrices_MotoYearId",
                table: "MotocrossMarketPrices",
                newName: "IX_MotocrossMarketPrices_YearId");

            migrationBuilder.RenameIndex(
                name: "IX_MotocrossMarketPrices_MotoMakeId",
                table: "MotocrossMarketPrices",
                newName: "IX_MotocrossMarketPrices_MakeId");

            migrationBuilder.CreateTable(
                name: "MotocrossEntries",
                columns: table => new
                {
                    Link = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MakeId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    Cc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotocrossEntries", x => x.Link);
                    table.ForeignKey(
                        name: "FK_MotocrossEntries_Makes_MakeId",
                        column: x => x.MakeId,
                        principalTable: "Makes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MotocrossEntries_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossEntries_MakeId",
                table: "MotocrossEntries",
                column: "MakeId");

            migrationBuilder.CreateIndex(
                name: "IX_MotocrossEntries_YearId",
                table: "MotocrossEntries",
                column: "YearId");

            migrationBuilder.AddForeignKey(
                name: "FK_MotocrossMarketPrices_Makes_MakeId",
                table: "MotocrossMarketPrices",
                column: "MakeId",
                principalTable: "Makes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MotocrossMarketPrices_Years_YearId",
                table: "MotocrossMarketPrices",
                column: "YearId",
                principalTable: "Years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotocrossMarketPrices_Makes_MakeId",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_MotocrossMarketPrices_Years_YearId",
                table: "MotocrossMarketPrices");

            migrationBuilder.DropTable(
                name: "MotocrossEntries");

            migrationBuilder.RenameColumn(
                name: "YearId",
                table: "MotocrossMarketPrices",
                newName: "MotoYearId");

            migrationBuilder.RenameColumn(
                name: "MakeId",
                table: "MotocrossMarketPrices",
                newName: "MotoMakeId");

            migrationBuilder.RenameIndex(
                name: "IX_MotocrossMarketPrices_YearId",
                table: "MotocrossMarketPrices",
                newName: "IX_MotocrossMarketPrices_MotoYearId");

            migrationBuilder.RenameIndex(
                name: "IX_MotocrossMarketPrices_MakeId",
                table: "MotocrossMarketPrices",
                newName: "IX_MotocrossMarketPrices_MotoMakeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MotocrossMarketPrices_Makes_MotoMakeId",
                table: "MotocrossMarketPrices",
                column: "MotoMakeId",
                principalTable: "Makes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MotocrossMarketPrices_Years_MotoYearId",
                table: "MotocrossMarketPrices",
                column: "MotoYearId",
                principalTable: "Years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
