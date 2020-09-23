using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class nulandoIDqb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdQB",
                table: "Habitaciones",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdQB",
                table: "Habitaciones",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
