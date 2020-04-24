using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambiosmetodobuscarorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Descuento",
                table: "Clientes",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Descuento",
                table: "Clientes",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
