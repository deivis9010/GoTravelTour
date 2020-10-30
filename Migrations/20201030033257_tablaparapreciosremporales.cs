using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class tablaparapreciosremporales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreciosOrdenModificados",
                columns: table => new
                {
                    PreciosOrdenModificadosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrdenId = table.Column<int>(nullable: false),
                    OrdenVehiculoId = table.Column<int>(nullable: false),
                    OrdenTrasladoId = table.Column<int>(nullable: false),
                    OrdenAlojamientoId = table.Column<int>(nullable: false),
                    OrdenActividadId = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Precio = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosOrdenModificados", x => x.PreciosOrdenModificadosId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreciosOrdenModificados");
        }
    }
}
