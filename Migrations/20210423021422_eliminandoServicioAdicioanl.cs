using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class eliminandoServicioAdicioanl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenServicioAdicional_ServicioAdicional_ServicioAdicionalId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropTable(
                name: "ServicioAdicional");

            migrationBuilder.DropIndex(
                name: "IX_OrdenServicioAdicional_ServicioAdicionalId",
                table: "OrdenServicioAdicional");

            migrationBuilder.AddColumn<string>(
                name: "Icono",
                table: "Productos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icono",
                table: "Productos");

            migrationBuilder.CreateTable(
                name: "ServicioAdicional",
                columns: table => new
                {
                    ServicioAdicionalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicioAdicional", x => x.ServicioAdicionalId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioAdicional_ServicioAdicionalId",
                table: "OrdenServicioAdicional",
                column: "ServicioAdicionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenServicioAdicional_ServicioAdicional_ServicioAdicionalId",
                table: "OrdenServicioAdicional",
                column: "ServicioAdicionalId",
                principalTable: "ServicioAdicional",
                principalColumn: "ServicioAdicionalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
