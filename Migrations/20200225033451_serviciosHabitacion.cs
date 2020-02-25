using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class serviciosHabitacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiciosHabitacion",
                columns: table => new
                {
                    ServiciosHabitacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosHabitacion", x => x.ServiciosHabitacionId);
                });

            migrationBuilder.CreateTable(
                name: "HabitacionServiciosHabitacion",
                columns: table => new
                {
                    HabitacionServiciosHabitacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HabitacionId = table.Column<int>(nullable: false),
                    ServiciosHabitacionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitacionServiciosHabitacion", x => x.HabitacionServiciosHabitacionId);
                    table.ForeignKey(
                        name: "FK_HabitacionServiciosHabitacion_Habitaciones_HabitacionId",
                        column: x => x.HabitacionId,
                        principalTable: "Habitaciones",
                        principalColumn: "HabitacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HabitacionServiciosHabitacion_ServiciosHabitacion_ServiciosHabitacionId",
                        column: x => x.ServiciosHabitacionId,
                        principalTable: "ServiciosHabitacion",
                        principalColumn: "ServiciosHabitacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionServiciosHabitacion_HabitacionId",
                table: "HabitacionServiciosHabitacion",
                column: "HabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitacionServiciosHabitacion_ServiciosHabitacionId",
                table: "HabitacionServiciosHabitacion",
                column: "ServiciosHabitacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HabitacionServiciosHabitacion");

            migrationBuilder.DropTable(
                name: "ServiciosHabitacion");
        }
    }
}
