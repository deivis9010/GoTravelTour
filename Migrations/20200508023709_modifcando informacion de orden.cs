using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modifcandoinformaciondeorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SobreprecioId",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorSobreprecioAplicado",
                table: "OrdenVehiculo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SobreprecioId",
                table: "OrdenTraslado",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorSobreprecioAplicado",
                table: "OrdenTraslado",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SobreprecioId",
                table: "OrdenAlojamiento",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorSobreprecioAplicado",
                table: "OrdenAlojamiento",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SobreprecioId",
                table: "OrdenActividad",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorSobreprecioAplicado",
                table: "OrdenActividad",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "Orden",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "Orden",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_SobreprecioId",
                table: "OrdenVehiculo",
                column: "SobreprecioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_SobreprecioId",
                table: "OrdenTraslado",
                column: "SobreprecioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_SobreprecioId",
                table: "OrdenAlojamiento",
                column: "SobreprecioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_SobreprecioId",
                table: "OrdenActividad",
                column: "SobreprecioId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_Sobreprecio_SobreprecioId",
                table: "OrdenActividad",
                column: "SobreprecioId",
                principalTable: "Sobreprecio",
                principalColumn: "SobreprecioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_Sobreprecio_SobreprecioId",
                table: "OrdenAlojamiento",
                column: "SobreprecioId",
                principalTable: "Sobreprecio",
                principalColumn: "SobreprecioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_Sobreprecio_SobreprecioId",
                table: "OrdenTraslado",
                column: "SobreprecioId",
                principalTable: "Sobreprecio",
                principalColumn: "SobreprecioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenVehiculo_Sobreprecio_SobreprecioId",
                table: "OrdenVehiculo",
                column: "SobreprecioId",
                principalTable: "Sobreprecio",
                principalColumn: "SobreprecioId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_Sobreprecio_SobreprecioId",
                table: "OrdenActividad");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_Sobreprecio_SobreprecioId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_Sobreprecio_SobreprecioId",
                table: "OrdenTraslado");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenVehiculo_Sobreprecio_SobreprecioId",
                table: "OrdenVehiculo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenVehiculo_SobreprecioId",
                table: "OrdenVehiculo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenTraslado_SobreprecioId",
                table: "OrdenTraslado");

            migrationBuilder.DropIndex(
                name: "IX_OrdenAlojamiento_SobreprecioId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropIndex(
                name: "IX_OrdenActividad_SobreprecioId",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "SobreprecioId",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "ValorSobreprecioAplicado",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "SobreprecioId",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "ValorSobreprecioAplicado",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "SobreprecioId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "ValorSobreprecioAplicado",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "SobreprecioId",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "ValorSobreprecioAplicado",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "Orden");
        }
    }
}
