using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambiasycategoriahoteles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Productos",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Clase",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoriaHoteles",
                columns: table => new
                {
                    CategoriaHotelesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaHoteles", x => x.CategoriaHotelesId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriaHoteles");

            migrationBuilder.DropColumn(
                name: "Clase",
                table: "Productos");

            migrationBuilder.AlterColumn<int>(
                name: "Categoria",
                table: "Productos",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
