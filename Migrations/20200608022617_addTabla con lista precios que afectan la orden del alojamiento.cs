using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addTablaconlistapreciosqueafectanlaordendelalojamiento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_PrecioAlojamiento_PrecioAlojamientoId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropIndex(
                name: "IX_OrdenAlojamiento_PrecioAlojamientoId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "PrecioAlojamientoId",
                table: "OrdenAlojamiento");

            migrationBuilder.CreateTable(
                name: "OrdenAlojamientoPrecioAlojamiento",
                columns: table => new
                {
                    OrdenAlojamientoPrecioAlojamientoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PrecioAlojamientoId = table.Column<int>(nullable: true),
                    OrdenAlojamientoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenAlojamientoPrecioAlojamiento", x => x.OrdenAlojamientoPrecioAlojamientoId);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamientoPrecioAlojamiento_OrdenAlojamiento_OrdenAlojamientoId",
                        column: x => x.OrdenAlojamientoId,
                        principalTable: "OrdenAlojamiento",
                        principalColumn: "OrdenAlojamientoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamientoPrecioAlojamiento_PrecioAlojamiento_PrecioAlojamientoId",
                        column: x => x.PrecioAlojamientoId,
                        principalTable: "PrecioAlojamiento",
                        principalColumn: "PrecioAlojamientoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamientoPrecioAlojamiento_OrdenAlojamientoId",
                table: "OrdenAlojamientoPrecioAlojamiento",
                column: "OrdenAlojamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamientoPrecioAlojamiento_PrecioAlojamientoId",
                table: "OrdenAlojamientoPrecioAlojamiento",
                column: "PrecioAlojamientoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenAlojamientoPrecioAlojamiento");

            migrationBuilder.AddColumn<int>(
                name: "PrecioAlojamientoId",
                table: "OrdenAlojamiento",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_PrecioAlojamientoId",
                table: "OrdenAlojamiento",
                column: "PrecioAlojamientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_PrecioAlojamiento_PrecioAlojamientoId",
                table: "OrdenAlojamiento",
                column: "PrecioAlojamientoId",
                principalTable: "PrecioAlojamiento",
                principalColumn: "PrecioAlojamientoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
