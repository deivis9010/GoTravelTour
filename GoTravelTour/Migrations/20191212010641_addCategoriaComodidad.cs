using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addCategoriaComodidad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaComodidadId",
                table: "Comodidades",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategoriaComodidad",
                columns: table => new
                {
                    CategoriaComodidadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaComodidad", x => x.CategoriaComodidadId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comodidades_CategoriaComodidadId",
                table: "Comodidades",
                column: "CategoriaComodidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comodidades_CategoriaComodidad_CategoriaComodidadId",
                table: "Comodidades",
                column: "CategoriaComodidadId",
                principalTable: "CategoriaComodidad",
                principalColumn: "CategoriaComodidadId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comodidades_CategoriaComodidad_CategoriaComodidadId",
                table: "Comodidades");

            migrationBuilder.DropTable(
                name: "CategoriaComodidad");

            migrationBuilder.DropIndex(
                name: "IX_Comodidades_CategoriaComodidadId",
                table: "Comodidades");

            migrationBuilder.DropColumn(
                name: "CategoriaComodidadId",
                table: "Comodidades");
        }
    }
}
