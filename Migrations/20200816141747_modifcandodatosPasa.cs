using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modifcandodatosPasa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AffiDavitOk",
                table: "DatosPasajeroSecundario",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AffiDavitOk",
                table: "DatosPasajeroPrimario",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AffiDavitOk",
                table: "DatosPasajeroSecundario");

            migrationBuilder.DropColumn(
                name: "AffiDavitOk",
                table: "DatosPasajeroPrimario");
        }
    }
}
