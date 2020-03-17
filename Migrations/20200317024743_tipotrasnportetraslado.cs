using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class tipotrasnportetraslado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoTransporteId",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TipoTransporteId",
                table: "Productos",
                column: "TipoTransporteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_TipoTransportes_TipoTransporteId",
                table: "Productos",
                column: "TipoTransporteId",
                principalTable: "TipoTransportes",
                principalColumn: "TipoTransporteId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_TipoTransportes_TipoTransporteId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_TipoTransporteId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoTransporteId",
                table: "Productos");
        }
    }
}
