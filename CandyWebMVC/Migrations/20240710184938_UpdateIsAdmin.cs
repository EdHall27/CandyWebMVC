using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CandyWebMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIsAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "User",
                defaultValue: false,
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
