using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addTablaconlistapreciosqueafectanlaordendelvehiculo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenVehiculo_PrecioRentaAutos_PrecioRentaAutosId",
                table: "OrdenVehiculo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenVehiculo_PrecioRentaAutosId",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "PrecioRentaAutosId",
                table: "OrdenVehiculo");

            migrationBuilder.CreateTable(
                name: "OrdenVehiculoPrecioRentaAuto",
                columns: table => new
                {
                    OrdenVehiculoPrecioRentaAutoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrdenVehiculoId = table.Column<int>(nullable: true),
                    PrecioRentaAutosId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenVehiculoPrecioRentaAuto", x => x.OrdenVehiculoPrecioRentaAutoId);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculoPrecioRentaAuto_OrdenVehiculo_OrdenVehiculoId",
                        column: x => x.OrdenVehiculoId,
                        principalTable: "OrdenVehiculo",
                        principalColumn: "OrdenVehiculoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculoPrecioRentaAuto_PrecioRentaAutos_PrecioRentaAutosId",
                        column: x => x.PrecioRentaAutosId,
                        principalTable: "PrecioRentaAutos",
                        principalColumn: "PrecioRentaAutosId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculoPrecioRentaAuto_OrdenVehiculoId",
                table: "OrdenVehiculoPrecioRentaAuto",
                column: "OrdenVehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculoPrecioRentaAuto_PrecioRentaAutosId",
                table: "OrdenVehiculoPrecioRentaAuto",
                column: "PrecioRentaAutosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenVehiculoPrecioRentaAuto");

            migrationBuilder.AddColumn<int>(
                name: "PrecioRentaAutosId",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_PrecioRentaAutosId",
                table: "OrdenVehiculo",
                column: "PrecioRentaAutosId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenVehiculo_PrecioRentaAutos_PrecioRentaAutosId",
                table: "OrdenVehiculo",
                column: "PrecioRentaAutosId",
                principalTable: "PrecioRentaAutos",
                principalColumn: "PrecioRentaAutosId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
