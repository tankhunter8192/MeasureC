using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gpib.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileDeviceIdToGPIBDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileDeviceId",
                table: "GPIBDevice",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileDeviceId",
                table: "GPIBDevice");
        }
    }
}
