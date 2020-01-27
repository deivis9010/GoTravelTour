using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class añadiendoplanesalimenticiosyalmacenimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pais",
                table: "Clientes",
                newName: "Direccion");

            migrationBuilder.AddColumn<string>(
                name: "Calle",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Clientes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AlmacenImagenes",
                columns: table => new
                {
                    AlmacenImagenesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NombreImagen = table.Column<string>(nullable: true),
                    Localizacion = table.Column<string>(nullable: true),
                    ImageContent = table.Column<string>(nullable: true),
                    TipoImagen = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlmacenImagenes", x => x.AlmacenImagenesId);
                });

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

            migrationBuilder.CreateTable(
                name: "PlanesAlimenticios",
                columns: table => new
                {
                    PlanesAlimenticiosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesAlimenticios", x => x.PlanesAlimenticiosId);
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Paises_PaisId",
                table: "Clientes");

            migrationBuilder.DropTable(
                name: "AlmacenImagenes");

            migrationBuilder.DropTable(
                name: "Paises");

            migrationBuilder.DropTable(
                name: "PlanesAlimenticios");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_PaisId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Calle",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Clientes",
                newName: "Pais");
        }
    }
}
