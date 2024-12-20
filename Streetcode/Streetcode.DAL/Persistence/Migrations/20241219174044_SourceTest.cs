using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streetcode.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SourceTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_streetcode_source_link_categories",
                schema: "sources",
                table: "streetcode_source_link_categories");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "sources",
                table: "streetcode_source_link_categories",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "sources",
                table: "streetcode_source_link_categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_streetcode_source_link_categories",
                schema: "sources",
                table: "streetcode_source_link_categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_streetcode_source_link_categories_SourceLinkCategoryId",
                schema: "sources",
                table: "streetcode_source_link_categories",
                column: "SourceLinkCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_streetcode_source_link_categories",
                schema: "sources",
                table: "streetcode_source_link_categories");

            migrationBuilder.DropIndex(
                name: "IX_streetcode_source_link_categories_SourceLinkCategoryId",
                schema: "sources",
                table: "streetcode_source_link_categories");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "sources",
                table: "streetcode_source_link_categories");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "sources",
                table: "streetcode_source_link_categories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_streetcode_source_link_categories",
                schema: "sources",
                table: "streetcode_source_link_categories",
                columns: new[] { "SourceLinkCategoryId", "StreetcodeId" });
        }
    }
}
