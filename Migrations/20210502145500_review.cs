using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class review : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoViajeBoleto",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "TipoViaje",
                table: "OrdenServicioAdicional",
                newName: "TipoViajeBoleto");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "OrdenServicioAdicional",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "OrdenServicioAdicional");

            migrationBuilder.RenameColumn(
                name: "TipoViajeBoleto",
                table: "OrdenServicioAdicional",
                newName: "TipoViaje");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoViajeBoleto",
                table: "Productos",
                nullable: true);
        }
    }
}
