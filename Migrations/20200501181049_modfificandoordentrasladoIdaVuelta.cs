using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modfificandoordentrasladoIdaVuelta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orden_Sobreprecio_SobreprecioId",
                table: "Orden");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_LugarEntregaPuntoInteresId",
                table: "OrdenTraslado");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_LugarRecogidaPuntoInteresId",
                table: "OrdenTraslado");

            migrationBuilder.DropIndex(
                name: "IX_Orden_SobreprecioId",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "SobreprecioId",
                table: "Orden");

            migrationBuilder.RenameColumn(
                name: "LugarRecogidaPuntoInteresId",
                table: "OrdenTraslado",
                newName: "PuntoOrigenPuntoInteresId");

            migrationBuilder.RenameColumn(
                name: "LugarEntregaPuntoInteresId",
                table: "OrdenTraslado",
                newName: "PuntoDestinoPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenTraslado_LugarRecogidaPuntoInteresId",
                table: "OrdenTraslado",
                newName: "IX_OrdenTraslado_PuntoOrigenPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenTraslado_LugarEntregaPuntoInteresId",
                table: "OrdenTraslado",
                newName: "IX_OrdenTraslado_PuntoDestinoPuntoInteresId");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Usuarios",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Usuarios",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<bool>(
                name: "IsIdaVuelta",
                table: "OrdenTraslado",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_PuntoDestinoPuntoInteresId",
                table: "OrdenTraslado",
                column: "PuntoDestinoPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_PuntoOrigenPuntoInteresId",
                table: "OrdenTraslado",
                column: "PuntoOrigenPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_PuntoDestinoPuntoInteresId",
                table: "OrdenTraslado");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_PuntoOrigenPuntoInteresId",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "IsIdaVuelta",
                table: "OrdenTraslado");

            migrationBuilder.RenameColumn(
                name: "PuntoOrigenPuntoInteresId",
                table: "OrdenTraslado",
                newName: "LugarRecogidaPuntoInteresId");

            migrationBuilder.RenameColumn(
                name: "PuntoDestinoPuntoInteresId",
                table: "OrdenTraslado",
                newName: "LugarEntregaPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenTraslado_PuntoOrigenPuntoInteresId",
                table: "OrdenTraslado",
                newName: "IX_OrdenTraslado_LugarRecogidaPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenTraslado_PuntoDestinoPuntoInteresId",
                table: "OrdenTraslado",
                newName: "IX_OrdenTraslado_LugarEntregaPuntoInteresId");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Usuarios",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Usuarios",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SobreprecioId",
                table: "Orden",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orden_SobreprecioId",
                table: "Orden",
                column: "SobreprecioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orden_Sobreprecio_SobreprecioId",
                table: "Orden",
                column: "SobreprecioId",
                principalTable: "Sobreprecio",
                principalColumn: "SobreprecioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_LugarEntregaPuntoInteresId",
                table: "OrdenTraslado",
                column: "LugarEntregaPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_PuntosInteres_LugarRecogidaPuntoInteresId",
                table: "OrdenTraslado",
                column: "LugarRecogidaPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
