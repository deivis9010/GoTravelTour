using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambiandotipodedato : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NombreCompania",
                table: "ConfiguracionVoucher",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "NombreCompania",
                table: "ConfiguracionVoucher",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
