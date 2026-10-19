using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoTrack.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryPreferenceAndMovementCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDate",
                table: "Cargos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryPreference",
                table: "Cargos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NeighborAddress",
                table: "Cargos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCity",
                table: "CargoMovements",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentDate",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "DeliveryPreference",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "NeighborAddress",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "LocationCity",
                table: "CargoMovements");
        }
    }
}
