using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace essSync.Migrations
{
    /// <inheritdoc />
    public partial class FilesAndFolderNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "SharedFolders",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumFiles",
                table: "SharedFolders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumSubFolders",
                table: "SharedFolders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "SharedFolders");

            migrationBuilder.DropColumn(
                name: "NumFiles",
                table: "SharedFolders");

            migrationBuilder.DropColumn(
                name: "NumSubFolders",
                table: "SharedFolders");
        }
    }
}
