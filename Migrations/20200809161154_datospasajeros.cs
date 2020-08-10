using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class datospasajeros : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatosPasajeroPrimario",
                columns: table => new
                {
                    DatosPasajeroPrimarioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Apellidos = table.Column<string>(nullable: true),
                    FechaNacimiento = table.Column<DateTime>(nullable: false),
                    CiudadSalida = table.Column<string>(nullable: true),
                    Nacionalidad = table.Column<string>(nullable: true),
                    NumeroPasaporte = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Direccion = table.Column<string>(nullable: true),
                    Calle = table.Column<string>(nullable: true),
                    Ciudad = table.Column<string>(nullable: true),
                    Estado = table.Column<string>(nullable: true),
                    CodigoPostal = table.Column<string>(nullable: true),
                    Pais = table.Column<string>(nullable: true),
                    AffiDavitRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosPasajeroPrimario", x => x.DatosPasajeroPrimarioId);
                });

            migrationBuilder.CreateTable(
                name: "DatosPasajeroSecundario",
                columns: table => new
                {
                    DatosPasajeroSecundarioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Apellidos = table.Column<string>(nullable: true),
                    FechaNacimiento = table.Column<DateTime>(nullable: false),
                    CiudadSalida = table.Column<string>(nullable: true),
                    Nacionalidad = table.Column<string>(nullable: true),
                    NumeroPasaporte = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Direccion = table.Column<string>(nullable: true),
                    Calle = table.Column<string>(nullable: true),
                    Ciudad = table.Column<string>(nullable: true),
                    Estado = table.Column<string>(nullable: true),
                    CodigoPostal = table.Column<string>(nullable: true),
                    Pais = table.Column<string>(nullable: true),
                    AffiDavitRequired = table.Column<bool>(nullable: false),
                    DatosPasajeroPrimarioId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosPasajeroSecundario", x => x.DatosPasajeroSecundarioId);
                    table.ForeignKey(
                        name: "FK_DatosPasajeroSecundario_DatosPasajeroPrimario_DatosPasajeroPrimarioId",
                        column: x => x.DatosPasajeroPrimarioId,
                        principalTable: "DatosPasajeroPrimario",
                        principalColumn: "DatosPasajeroPrimarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatosPasajeroSecundario_DatosPasajeroPrimarioId",
                table: "DatosPasajeroSecundario",
                column: "DatosPasajeroPrimarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatosPasajeroSecundario");

            migrationBuilder.DropTable(
                name: "DatosPasajeroPrimario");
        }
    }
}
