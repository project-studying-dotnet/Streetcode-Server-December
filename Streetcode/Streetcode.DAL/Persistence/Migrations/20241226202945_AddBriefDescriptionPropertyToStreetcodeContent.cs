using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streetcode.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBriefDescriptionPropertyToStreetcodeContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BriefDescription",
                schema: "streetcode",
                table: "streetcodes",
                type: "nvarchar(33)",
                maxLength: 33,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BriefDescription",
                schema: "streetcode",
                table: "streetcodes");
        }
    }
}
