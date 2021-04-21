using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addOrdenServicio2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdenServicioAdicional",
                columns: table => new
                {
                    OrdenServicioAdicionalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServicioAdicionalId = table.Column<int>(nullable: false),
                    CantidadAdultos = table.Column<int>(nullable: false),
                    CantidadNinos = table.Column<int>(nullable: false),
                    CantidadInfantes = table.Column<int>(nullable: false),
                    PrecioAdultos = table.Column<decimal>(nullable: false),
                    PrecioNinos = table.Column<decimal>(nullable: false),
                    PrecioInfantes = table.Column<decimal>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    TipoViaje = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenServicioAdicional", x => x.OrdenServicioAdicionalId);
                    table.ForeignKey(
                        name: "FK_OrdenServicioAdicional_ServicioAdicional_ServicioAdicionalId",
                        column: x => x.ServicioAdicionalId,
                        principalTable: "ServicioAdicional",
                        principalColumn: "ServicioAdicionalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioAdicional_ServicioAdicionalId",
                table: "OrdenServicioAdicional",
                column: "ServicioAdicionalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenServicioAdicional");
        }
    }
}
