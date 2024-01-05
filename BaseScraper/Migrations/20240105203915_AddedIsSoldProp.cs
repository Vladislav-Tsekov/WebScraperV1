using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseScraper.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsSoldProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "MotocrossEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "MotocrossEntries");
        }
    }
}
