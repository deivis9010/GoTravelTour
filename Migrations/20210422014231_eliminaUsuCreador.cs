using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class eliminaUsuCreador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orden_Usuarios_UsuarioCreadorUsuarioId",
                table: "Orden");

            migrationBuilder.DropIndex(
                name: "IX_Orden_UsuarioCreadorUsuarioId",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "UsuarioCreadorUsuarioId",
                table: "Orden");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreadorUsuarioId",
                table: "Orden",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orden_UsuarioCreadorUsuarioId",
                table: "Orden",
                column: "UsuarioCreadorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orden_Usuarios_UsuarioCreadorUsuarioId",
                table: "Orden",
                column: "UsuarioCreadorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
