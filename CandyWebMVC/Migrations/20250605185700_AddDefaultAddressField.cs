using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CandyWebMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAddressField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Address",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)); // ou DateTime.Now para um valor inicial

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Address",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Address",
                type: "bit", // ou bool
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Address");
        }
    }
}
