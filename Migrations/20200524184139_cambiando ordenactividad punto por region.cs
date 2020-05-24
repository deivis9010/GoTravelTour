using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambiandoordenactividadpuntoporregion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarActividadPuntoInteresId",
                table: "OrdenActividad");

            migrationBuilder.RenameColumn(
                name: "LugarActividadPuntoInteresId",
                table: "OrdenActividad",
                newName: "LugarActividadRegionId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenActividad_LugarActividadPuntoInteresId",
                table: "OrdenActividad",
                newName: "IX_OrdenActividad_LugarActividadRegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_Regiones_LugarActividadRegionId",
                table: "OrdenActividad",
                column: "LugarActividadRegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_Regiones_LugarActividadRegionId",
                table: "OrdenActividad");

            migrationBuilder.RenameColumn(
                name: "LugarActividadRegionId",
                table: "OrdenActividad",
                newName: "LugarActividadPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenActividad_LugarActividadRegionId",
                table: "OrdenActividad",
                newName: "IX_OrdenActividad_LugarActividadPuntoInteresId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_PuntosInteres_LugarActividadPuntoInteresId",
                table: "OrdenActividad",
                column: "LugarActividadPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
