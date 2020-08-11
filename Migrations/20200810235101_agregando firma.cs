using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class agregandofirma : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmacionOrden",
                table: "Orden",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageContent",
                table: "DatosPasajeroSecundario",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreImagen",
                table: "DatosPasajeroSecundario",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContent",
                table: "DatosPasajeroPrimario",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreImagen",
                table: "DatosPasajeroPrimario",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmacionOrden",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "ImageContent",
                table: "DatosPasajeroSecundario");

            migrationBuilder.DropColumn(
                name: "NombreImagen",
                table: "DatosPasajeroSecundario");

            migrationBuilder.DropColumn(
                name: "ImageContent",
                table: "DatosPasajeroPrimario");

            migrationBuilder.DropColumn(
                name: "NombreImagen",
                table: "DatosPasajeroPrimario");
        }
    }
}
