using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addproductorangofeche : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "RangoFechas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RangoFechas_ProductoId",
                table: "RangoFechas",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_RangoFechas_Productos_ProductoId",
                table: "RangoFechas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RangoFechas_Productos_ProductoId",
                table: "RangoFechas");

            migrationBuilder.DropIndex(
                name: "IX_RangoFechas_ProductoId",
                table: "RangoFechas");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "RangoFechas");
        }
    }
}
