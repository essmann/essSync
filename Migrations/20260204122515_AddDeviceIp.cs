using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace essSync.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Devices_DeviceGuid",
                table: "Devices",
                column: "DeviceGuid");

            migrationBuilder.CreateTable(
                name: "DeviceIps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceGuid = table.Column<string>(type: "TEXT", nullable: false),
                    Ip = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceIps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceIps_Devices_DeviceGuid",
                        column: x => x.DeviceGuid,
                        principalTable: "Devices",
                        principalColumn: "DeviceGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceIps_DeviceGuid",
                table: "DeviceIps",
                column: "DeviceGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceIps");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Devices_DeviceGuid",
                table: "Devices");
        }
    }
}
