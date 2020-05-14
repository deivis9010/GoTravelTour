using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class checkCambios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "OrdenVehiculo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "OrdenVehiculo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "OrdenTraslado",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "OrdenTraslado",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "OrdenAlojamiento",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "OrdenAlojamiento",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "OrdenActividad",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "OrdenActividad",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LugarActividadPuntoInteresId",
                table: "OrdenActividad",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LugarRecogidaPuntoInteresId",
                table: "OrdenActividad",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LugarRetornoPuntoInteresId",
                table: "OrdenActividad",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_LugarActividadPuntoInteresId",
                table: "OrdenActividad",
                column: "LugarActividadPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_LugarRecogidaPuntoInteresId",
                table: "OrdenActividad",
                column: "LugarRecogidaPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_LugarRetornoPuntoInteresId",
                table: "OrdenActividad",
                column: "LugarRetornoPuntoInteresId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarActividadPuntoInteresId",
                table: "OrdenActividad",
                column: "LugarActividadPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarRecogidaPuntoInteresId",
                table: "OrdenActividad",
                column: "LugarRecogidaPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarRetornoPuntoInteresId",
                table: "OrdenActividad",
                column: "LugarRetornoPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarActividadPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarRecogidaPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarRetornoPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropIndex(
                name: "IX_OrdenActividad_LugarActividadPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropIndex(
                name: "IX_OrdenActividad_LugarRecogidaPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropIndex(
                name: "IX_OrdenActividad_LugarRetornoPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "LugarActividadPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "LugarRecogidaPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "LugarRetornoPuntoInteresId",
                table: "OrdenActividad");
        }
    }
}
