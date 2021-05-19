using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class nuevoscampos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PorDia",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorSobreprecioAplicadoNeto",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PorDia",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "ValorSobreprecioAplicadoNeto",
                table: "OrdenServicioAdicional");
        }
    }
}
