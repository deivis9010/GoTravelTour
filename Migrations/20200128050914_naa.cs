﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class naa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacidadTraslado",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ModeloTraslado",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TrasladoId",
                table: "Productos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapacidadTraslado",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeloTraslado",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrasladoId",
                table: "Productos",
                nullable: true);
        }
    }
}
