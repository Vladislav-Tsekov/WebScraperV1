using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseScraper.Migrations
{
    /// <inheritdoc />
    public partial class AddedPriceChangeTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "MotocrossEntries",
                comment: "Table of all Motocross announcements");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "MotocrossEntries",
                type: "float",
                nullable: false,
                comment: "Motorcycle's actual price",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSold",
                table: "MotocrossEntries",
                type: "bit",
                nullable: false,
                comment: "Keeps track whether the motorcycle has been sold",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                table: "MotocrossEntries",
                type: "datetime2",
                nullable: false,
                comment: "Date of announcement's addition to the database",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Cc",
                table: "MotocrossEntries",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Motorcycle's engine displacement",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "MotocrossEntries",
                type: "nvarchar(450)",
                nullable: false,
                comment: "Each announcement link is unique, therefore used as a key",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<double>(
                name: "OldPrice",
                table: "MotocrossEntries",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                comment: "The price's value before it was changed");

            migrationBuilder.AddColumn<int>(
                name: "PriceChanges",
                table: "MotocrossEntries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Announcement's number of price changes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "MotocrossEntries");

            migrationBuilder.DropColumn(
                name: "PriceChanges",
                table: "MotocrossEntries");

            migrationBuilder.AlterTable(
                name: "MotocrossEntries",
                oldComment: "Table of all Motocross announcements");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "MotocrossEntries",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Motorcycle's actual price");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSold",
                table: "MotocrossEntries",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Keeps track whether the motorcycle has been sold");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                table: "MotocrossEntries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date of announcement's addition to the database");

            migrationBuilder.AlterColumn<string>(
                name: "Cc",
                table: "MotocrossEntries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Motorcycle's engine displacement");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "MotocrossEntries",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "Each announcement link is unique, therefore used as a key");
        }
    }
}
