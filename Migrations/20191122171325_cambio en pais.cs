using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambioenpais : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Paises_PaisId",
                table: "Clientes");

            migrationBuilder.DropTable(
                name: "Paises");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_PaisId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Clientes");

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Clientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Clientes");

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    PaisId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NombreCorto = table.Column<string>(nullable: true),
                    NombreLargo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.PaisId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_PaisId",
                table: "Clientes",
                column: "PaisId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Paises_PaisId",
                table: "Clientes",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "PaisId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
