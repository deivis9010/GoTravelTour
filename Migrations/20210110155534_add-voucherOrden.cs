using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addvoucherOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VoucherOrden",
                columns: table => new
                {
                    VoucherOrdenId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrdenId = table.Column<int>(nullable: false),
                    OrdenVehiculoId = table.Column<int>(nullable: false),
                    OrdenTrasladoId = table.Column<int>(nullable: false),
                    OrdenAlojamientoId = table.Column<int>(nullable: false),
                    OrdenActividadId = table.Column<int>(nullable: false),
                    UrlVoucher = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherOrden", x => x.VoucherOrdenId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherOrden");
        }
    }
}
