using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class nuevotipodeproducto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicioAdicionalId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoServicioAdicionalId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoViajeBoleto",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TipoServicioAdicional",
                columns: table => new
                {
                    TipoServicioAdicionalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoServicioAdicional", x => x.TipoServicioAdicionalId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TipoServicioAdicionalId",
                table: "Productos",
                column: "TipoServicioAdicionalId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioAdicional_ServicioAdicionalId",
                table: "OrdenServicioAdicional",
                column: "ServicioAdicionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenServicioAdicional_Productos_ServicioAdicionalId",
                table: "OrdenServicioAdicional",
                column: "ServicioAdicionalId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_TipoServicioAdicional_TipoServicioAdicionalId",
                table: "Productos",
                column: "TipoServicioAdicionalId",
                principalTable: "TipoServicioAdicional",
                principalColumn: "TipoServicioAdicionalId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenServicioAdicional_Productos_ServicioAdicionalId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_TipoServicioAdicional_TipoServicioAdicionalId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "TipoServicioAdicional");

            migrationBuilder.DropIndex(
                name: "IX_Productos_TipoServicioAdicionalId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_OrdenServicioAdicional_ServicioAdicionalId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ServicioAdicionalId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoServicioAdicionalId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoViajeBoleto",
                table: "Productos");
        }
    }
}
