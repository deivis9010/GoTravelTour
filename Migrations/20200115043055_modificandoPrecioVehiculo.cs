using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandoPrecioVehiculo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "PrecioRentaAutos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Seguro",
                table: "PrecioRentaAutos",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<decimal>(
                name: "Deposito",
                table: "PrecioRentaAutos",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deposito",
                table: "PrecioRentaAutos");

            migrationBuilder.AlterColumn<double>(
                name: "Seguro",
                table: "PrecioRentaAutos",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<double>(
                name: "Precio",
                table: "PrecioRentaAutos",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
