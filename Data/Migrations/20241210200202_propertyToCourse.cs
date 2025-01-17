using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciencesTechnology.Data.Migrations
{
    /// <inheritdoc />
    public partial class propertyToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureCourse",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureCourse",
                table: "Courses");
        }
    }
}
