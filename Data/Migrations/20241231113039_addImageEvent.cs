using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciencesTechnology.Data.Migrations
{
    /// <inheritdoc />
    public partial class addImageEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventImagePath",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventImagePath",
                table: "Events");
        }
    }
}
