using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class logoProveedor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageContent",
                table: "Proveedores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageMimeType",
                table: "Proveedores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Proveedores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContent",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ImageMimeType",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Proveedores");
        }
    }
}
