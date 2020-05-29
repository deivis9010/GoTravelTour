using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class agregandoPrecioPlanAlimetiocAOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrecioPlanesAlimenticiosId",
                table: "OrdenAlojamiento",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_PrecioPlanesAlimenticiosId",
                table: "OrdenAlojamiento",
                column: "PrecioPlanesAlimenticiosId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_PrecioPlanesAlimenticios_PrecioPlanesAlimenticiosId",
                table: "OrdenAlojamiento",
                column: "PrecioPlanesAlimenticiosId",
                principalTable: "PrecioPlanesAlimenticios",
                principalColumn: "PrecioPlanesAlimenticiosId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_PrecioPlanesAlimenticios_PrecioPlanesAlimenticiosId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropIndex(
                name: "IX_OrdenAlojamiento_PrecioPlanesAlimenticiosId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "PrecioPlanesAlimenticiosId",
                table: "OrdenAlojamiento");
        }
    }
}
