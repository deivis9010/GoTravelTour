using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class categoriaHoteles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaHotelesId",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaHotelesId",
                table: "Productos",
                column: "CategoriaHotelesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_CategoriaHoteles_CategoriaHotelesId",
                table: "Productos",
                column: "CategoriaHotelesId",
                principalTable: "CategoriaHoteles",
                principalColumn: "CategoriaHotelesId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_CategoriaHoteles_CategoriaHotelesId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_CategoriaHotelesId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CategoriaHotelesId",
                table: "Productos");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Productos",
                nullable: true);
        }
    }
}
