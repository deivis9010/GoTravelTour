using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class rutasregionpuntointeres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Regiones",
                columns: table => new
                {
                    RegionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regiones", x => x.RegionId);
                });

            migrationBuilder.CreateTable(
                name: "PuntosInteres",
                columns: table => new
                {
                    PuntoInteresId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    RegionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntosInteres", x => x.PuntoInteresId);
                    table.ForeignKey(
                        name: "FK_PuntosInteres_Regiones_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regiones",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rutas",
                columns: table => new
                {
                    RutasId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    regionOrigenRegionId = table.Column<int>(nullable: true),
                    regionDestinoRegionId = table.Column<int>(nullable: true),
                    puntoOrigenPuntoInteresId = table.Column<int>(nullable: true),
                    puntoDestinoPuntoInteresId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rutas", x => x.RutasId);
                    table.ForeignKey(
                        name: "FK_Rutas_PuntosInteres_puntoDestinoPuntoInteresId",
                        column: x => x.puntoDestinoPuntoInteresId,
                        principalTable: "PuntosInteres",
                        principalColumn: "PuntoInteresId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rutas_PuntosInteres_puntoOrigenPuntoInteresId",
                        column: x => x.puntoOrigenPuntoInteresId,
                        principalTable: "PuntosInteres",
                        principalColumn: "PuntoInteresId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rutas_Regiones_regionDestinoRegionId",
                        column: x => x.regionDestinoRegionId,
                        principalTable: "Regiones",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rutas_Regiones_regionOrigenRegionId",
                        column: x => x.regionOrigenRegionId,
                        principalTable: "Regiones",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PuntosInteres_RegionId",
                table: "PuntosInteres",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_puntoDestinoPuntoInteresId",
                table: "Rutas",
                column: "puntoDestinoPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_puntoOrigenPuntoInteresId",
                table: "Rutas",
                column: "puntoOrigenPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_regionDestinoRegionId",
                table: "Rutas",
                column: "regionDestinoRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_regionOrigenRegionId",
                table: "Rutas",
                column: "regionOrigenRegionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rutas");

            migrationBuilder.DropTable(
                name: "PuntosInteres");

            migrationBuilder.DropTable(
                name: "Regiones");
        }
    }
}
