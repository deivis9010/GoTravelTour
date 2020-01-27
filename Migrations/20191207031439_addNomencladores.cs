using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addNomencladores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Usuarios",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NombreTemporadas",
                columns: table => new
                {
                    NombreTemporadaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NombreTemporadas", x => x.NombreTemporadaId);
                });

            migrationBuilder.CreateTable(
                name: "TipoAlojamientos",
                columns: table => new
                {
                    TipoAlojamientoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoAlojamientos", x => x.TipoAlojamientoId);
                });

            migrationBuilder.CreateTable(
                name: "TipoHabitaciones",
                columns: table => new
                {
                    TipoHabitacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoHabitaciones", x => x.TipoHabitacionId);
                });

            migrationBuilder.CreateTable(
                name: "TipoProductos",
                columns: table => new
                {
                    TipoProductoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProductos", x => x.TipoProductoId);
                });

            migrationBuilder.CreateTable(
                name: "TipoTransportes",
                columns: table => new
                {
                    TipoTransporteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoTransportes", x => x.TipoTransporteId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NombreTemporadas");

            migrationBuilder.DropTable(
                name: "TipoAlojamientos");

            migrationBuilder.DropTable(
                name: "TipoHabitaciones");

            migrationBuilder.DropTable(
                name: "TipoProductos");

            migrationBuilder.DropTable(
                name: "TipoTransportes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Usuarios");
        }
    }
}
