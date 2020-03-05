using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambioshabitaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habitaciones_TipoHabitaciones_TipoHabitacionId",
                table: "Habitaciones");

            migrationBuilder.DropIndex(
                name: "IX_Habitaciones_TipoHabitacionId",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "TipoHabitacionId",
                table: "Habitaciones");

            migrationBuilder.AddColumn<bool>(
                name: "IsActivo",
                table: "Paquete",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CategoriaHabitacion",
                columns: table => new
                {
                    CategoriaHabitacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaHabitacion", x => x.CategoriaHabitacionId);
                });

            migrationBuilder.CreateTable(
                name: "HabitacionTipoHabitacion",
                columns: table => new
                {
                    HabitacionTipoHabitacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HabitacionId = table.Column<int>(nullable: false),
                    TipoHabitacionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitacionTipoHabitacion", x => x.HabitacionTipoHabitacionId);
                    table.ForeignKey(
                        name: "FK_HabitacionTipoHabitacion_Habitaciones_HabitacionId",
                        column: x => x.HabitacionId,
                        principalTable: "Habitaciones",
                        principalColumn: "HabitacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HabitacionTipoHabitacion_TipoHabitaciones_TipoHabitacionId",
                        column: x => x.TipoHabitacionId,
                        principalTable: "TipoHabitaciones",
                        principalColumn: "TipoHabitacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionTipoHabitacion_HabitacionId",
                table: "HabitacionTipoHabitacion",
                column: "HabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionTipoHabitacion_TipoHabitacionId",
                table: "HabitacionTipoHabitacion",
                column: "TipoHabitacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriaHabitacion");

            migrationBuilder.DropTable(
                name: "HabitacionTipoHabitacion");

            migrationBuilder.DropColumn(
                name: "IsActivo",
                table: "Paquete");

            migrationBuilder.AddColumn<int>(
                name: "TipoHabitacionId",
                table: "Habitaciones",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_TipoHabitacionId",
                table: "Habitaciones",
                column: "TipoHabitacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habitaciones_TipoHabitaciones_TipoHabitacionId",
                table: "Habitaciones",
                column: "TipoHabitacionId",
                principalTable: "TipoHabitaciones",
                principalColumn: "TipoHabitacionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
