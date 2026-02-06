using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace essSync.Migrations
{
    /// <inheritdoc />
    public partial class addFolderPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                table: "SharedFolders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "SharedFolders");
        }
    }
}
