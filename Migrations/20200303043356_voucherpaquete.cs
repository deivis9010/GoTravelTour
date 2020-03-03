using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class voucherpaquete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlojamientosPlanesAlimenticios",
                columns: table => new
                {
                    AlojamientosPlanesAlimenticiosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlojamientoId = table.Column<int>(nullable: false),
                    PlanesAlimenticiosId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlojamientosPlanesAlimenticios", x => x.AlojamientosPlanesAlimenticiosId);
                    table.ForeignKey(
                        name: "FK_AlojamientosPlanesAlimenticios_Productos_AlojamientoId",
                        column: x => x.AlojamientoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlojamientosPlanesAlimenticios_PlanesAlimenticios_PlanesAlimenticiosId",
                        column: x => x.PlanesAlimenticiosId,
                        principalTable: "PlanesAlimenticios",
                        principalColumn: "PlanesAlimenticiosId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionVoucher",
                columns: table => new
                {
                    ConfiguracionVoucherId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TipoProductoId = table.Column<int>(nullable: false),
                    Condiciones = table.Column<string>(nullable: true),
                    InfoAgente = table.Column<string>(nullable: true),
                    TelefonoAsistencia = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true),
                    ImageContent = table.Column<string>(nullable: true),
                    TipoImagen = table.Column<string>(nullable: true),
                    IsActivo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionVoucher", x => x.ConfiguracionVoucherId);
                    table.ForeignKey(
                        name: "FK_ConfiguracionVoucher_TipoProductos_TipoProductoId",
                        column: x => x.TipoProductoId,
                        principalTable: "TipoProductos",
                        principalColumn: "TipoProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Paquete",
                columns: table => new
                {
                    PaqueteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    ImageContentP = table.Column<string>(nullable: true),
                    TipoImagenP = table.Column<string>(nullable: true),
                    ImageContent1 = table.Column<string>(nullable: true),
                    TipoImagen1 = table.Column<string>(nullable: true),
                    ImageContent2 = table.Column<string>(nullable: true),
                    TipoImagen2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paquete", x => x.PaqueteId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlojamientosPlanesAlimenticios_AlojamientoId",
                table: "AlojamientosPlanesAlimenticios",
                column: "AlojamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_AlojamientosPlanesAlimenticios_PlanesAlimenticiosId",
                table: "AlojamientosPlanesAlimenticios",
                column: "PlanesAlimenticiosId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionVoucher_TipoProductoId",
                table: "ConfiguracionVoucher",
                column: "TipoProductoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlojamientosPlanesAlimenticios");

            migrationBuilder.DropTable(
                name: "ConfiguracionVoucher");

            migrationBuilder.DropTable(
                name: "Paquete");
        }
    }
}
