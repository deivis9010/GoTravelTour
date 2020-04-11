using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class test22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orden",
                columns: table => new
                {
                    OrdenId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(nullable: true),
                    NumeroOrden = table.Column<string>(nullable: true),
                    Duracion = table.Column<int>(nullable: false),
                    Nombre = table.Column<string>(nullable: true),
                    ClienteId = table.Column<int>(nullable: false),
                    OFACrequired = table.Column<bool>(nullable: false),
                    HasVoucher = table.Column<bool>(nullable: false),
                    Notas = table.Column<string>(nullable: true),
                    IntercomConferceNumber = table.Column<string>(nullable: true),
                    CreadorUsuarioId = table.Column<int>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    FechaActualizacion = table.Column<DateTime>(nullable: true),
                    SobreprecioId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orden", x => x.OrdenId);
                    table.ForeignKey(
                        name: "FK_Orden_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orden_Usuarios_CreadorUsuarioId",
                        column: x => x.CreadorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orden_Sobreprecio_SobreprecioId",
                        column: x => x.SobreprecioId,
                        principalTable: "Sobreprecio",
                        principalColumn: "SobreprecioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenProducto",
                columns: table => new
                {
                    OrdenProductoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: false),
                    OrdenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenProducto", x => x.OrdenProductoId);
                    table.ForeignKey(
                        name: "FK_OrdenProducto_Orden_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "Orden",
                        principalColumn: "OrdenId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenProducto_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orden_ClienteId",
                table: "Orden",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Orden_CreadorUsuarioId",
                table: "Orden",
                column: "CreadorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Orden_SobreprecioId",
                table: "Orden",
                column: "SobreprecioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenProducto_OrdenId",
                table: "OrdenProducto",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenProducto_ProductoId",
                table: "OrdenProducto",
                column: "ProductoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenProducto");

            migrationBuilder.DropTable(
                name: "Orden");
        }
    }
}
