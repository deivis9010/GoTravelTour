using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class poniendononullCategoiIDEnalohjamiento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_CategoriaHoteles_CategoriaHotelesId",
                table: "Productos");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_CategoriaHoteles_CategoriaHotelesId",
                table: "Productos",
                column: "CategoriaHotelesId",
                principalTable: "CategoriaHoteles",
                principalColumn: "CategoriaHotelesId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_CategoriaHoteles_CategoriaHotelesId",
                table: "Productos");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_CategoriaHoteles_CategoriaHotelesId",
                table: "Productos",
                column: "CategoriaHotelesId",
                principalTable: "CategoriaHoteles",
                principalColumn: "CategoriaHotelesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
