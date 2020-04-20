using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class primeraversionordenes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenProducto");

            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Orden");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Orden",
                newName: "NumeroAsistencia");

            migrationBuilder.AddColumn<int>(
                name: "VehiculoProductoId",
                table: "VehiculoCategoriaAuto",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPeticion",
                table: "Orden",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreClienteFinal",
                table: "Orden",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreOrden",
                table: "Orden",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrdenActividad",
                columns: table => new
                {
                    OrdenActividadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FechaActividad = table.Column<DateTime>(nullable: false),
                    CantAdulto = table.Column<int>(nullable: true),
                    CantNino = table.Column<int>(nullable: true),
                    CantInfante = table.Column<int>(nullable: true),
                    TelefonoReferencia = table.Column<string>(nullable: true),
                    DireccionReferencia = table.Column<string>(nullable: true),
                    NombreCliente = table.Column<string>(nullable: true),
                    VenueName = table.Column<string>(nullable: true),
                    NumeroConfirmacion = table.Column<string>(nullable: true),
                    DescripcionServicio = table.Column<string>(nullable: true),
                    NotasAdicionales = table.Column<string>(nullable: true),
                    DistribuidorId = table.Column<int>(nullable: false),
                    ActividadId = table.Column<int>(nullable: false),
                    PrecioActividadId = table.Column<int>(nullable: true),
                    PremiteCopia = table.Column<bool>(nullable: false),
                    OrdenId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenActividad", x => x.OrdenActividadId);
                    table.ForeignKey(
                        name: "FK_OrdenActividad_Productos_ActividadId",
                        column: x => x.ActividadId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenActividad_Distribuidores_DistribuidorId",
                        column: x => x.DistribuidorId,
                        principalTable: "Distribuidores",
                        principalColumn: "DistribuidorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenActividad_Orden_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "Orden",
                        principalColumn: "OrdenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenActividad_PrecioActividad_PrecioActividadId",
                        column: x => x.PrecioActividadId,
                        principalTable: "PrecioActividad",
                        principalColumn: "PrecioActividadId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenAlojamiento",
                columns: table => new
                {
                    OrdenAlojamientoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CantAdulto = table.Column<int>(nullable: true),
                    CantNino = table.Column<int>(nullable: true),
                    CantInfante = table.Column<int>(nullable: true),
                    TelefonoReferencia = table.Column<string>(nullable: true),
                    DireccionReferencia = table.Column<string>(nullable: true),
                    NombreCliente = table.Column<string>(nullable: true),
                    VenueName = table.Column<string>(nullable: true),
                    NumeroConfirmacion = table.Column<string>(nullable: true),
                    DescripcionServicio = table.Column<string>(nullable: true),
                    NotasAdicionales = table.Column<string>(nullable: true),
                    Checkin = table.Column<DateTime>(nullable: false),
                    Checkout = table.Column<DateTime>(nullable: false),
                    ConfiguracionVoucherId = table.Column<int>(nullable: true),
                    PrecioAlojamientoId = table.Column<int>(nullable: true),
                    HabitacionId = table.Column<int>(nullable: true),
                    DistribuidorId = table.Column<int>(nullable: false),
                    PlanesAlimenticiosId = table.Column<int>(nullable: false),
                    AlojamientoId = table.Column<int>(nullable: false),
                    PremiteCopia = table.Column<bool>(nullable: false),
                    OrdenId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenAlojamiento", x => x.OrdenAlojamientoId);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamiento_Productos_AlojamientoId",
                        column: x => x.AlojamientoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamiento_ConfiguracionVoucher_ConfiguracionVoucherId",
                        column: x => x.ConfiguracionVoucherId,
                        principalTable: "ConfiguracionVoucher",
                        principalColumn: "ConfiguracionVoucherId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamiento_Distribuidores_DistribuidorId",
                        column: x => x.DistribuidorId,
                        principalTable: "Distribuidores",
                        principalColumn: "DistribuidorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamiento_Habitaciones_HabitacionId",
                        column: x => x.HabitacionId,
                        principalTable: "Habitaciones",
                        principalColumn: "HabitacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamiento_Orden_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "Orden",
                        principalColumn: "OrdenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamiento_PlanesAlimenticios_PlanesAlimenticiosId",
                        column: x => x.PlanesAlimenticiosId,
                        principalTable: "PlanesAlimenticios",
                        principalColumn: "PlanesAlimenticiosId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenAlojamiento_PrecioAlojamiento_PrecioAlojamientoId",
                        column: x => x.PrecioAlojamientoId,
                        principalTable: "PrecioAlojamiento",
                        principalColumn: "PrecioAlojamientoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenTraslado",
                columns: table => new
                {
                    OrdenTrasladoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CantAdulto = table.Column<int>(nullable: true),
                    CantNino = table.Column<int>(nullable: true),
                    CantInfante = table.Column<int>(nullable: true),
                    FechaRecogida = table.Column<DateTime>(nullable: false),
                    NumeroConfirmacion = table.Column<string>(nullable: true),
                    InformacionLlegada = table.Column<string>(nullable: true),
                    NombreCliente = table.Column<string>(nullable: true),
                    InformacionSalida = table.Column<string>(nullable: true),
                    DescripcionServicio = table.Column<string>(nullable: true),
                    PrecioTrasladoId = table.Column<int>(nullable: true),
                    ConfiguracionVoucherId = table.Column<int>(nullable: true),
                    TipoTraslado = table.Column<string>(nullable: true),
                    DistribuidorId = table.Column<int>(nullable: false),
                    LugarRecogidaPuntoInteresId = table.Column<int>(nullable: true),
                    LugarEntregaPuntoInteresId = table.Column<int>(nullable: true),
                    TrasladoProductoId = table.Column<int>(nullable: true),
                    PremiteCopia = table.Column<bool>(nullable: false),
                    OrdenId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenTraslado", x => x.OrdenTrasladoId);
                    table.ForeignKey(
                        name: "FK_OrdenTraslado_ConfiguracionVoucher_ConfiguracionVoucherId",
                        column: x => x.ConfiguracionVoucherId,
                        principalTable: "ConfiguracionVoucher",
                        principalColumn: "ConfiguracionVoucherId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenTraslado_Distribuidores_DistribuidorId",
                        column: x => x.DistribuidorId,
                        principalTable: "Distribuidores",
                        principalColumn: "DistribuidorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenTraslado_PuntosInteres_LugarEntregaPuntoInteresId",
                        column: x => x.LugarEntregaPuntoInteresId,
                        principalTable: "PuntosInteres",
                        principalColumn: "PuntoInteresId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenTraslado_PuntosInteres_LugarRecogidaPuntoInteresId",
                        column: x => x.LugarRecogidaPuntoInteresId,
                        principalTable: "PuntosInteres",
                        principalColumn: "PuntoInteresId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenTraslado_Orden_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "Orden",
                        principalColumn: "OrdenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenTraslado_PrecioTraslados_PrecioTrasladoId",
                        column: x => x.PrecioTrasladoId,
                        principalTable: "PrecioTraslados",
                        principalColumn: "PrecioTrasladoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenTraslado_Productos_TrasladoProductoId",
                        column: x => x.TrasladoProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenVehiculo",
                columns: table => new
                {
                    OrdenVehiculoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NumeroConfirmacion = table.Column<string>(nullable: true),
                    NombreCliente = table.Column<string>(nullable: true),
                    FechaRecogida = table.Column<DateTime>(nullable: false),
                    FechaEntrega = table.Column<DateTime>(nullable: false),
                    LugarRecogidaPuntoInteresId = table.Column<int>(nullable: true),
                    LugarEntregaPuntoInteresId = table.Column<int>(nullable: true),
                    DistribuidorId = table.Column<int>(nullable: false),
                    PrecioRentaAutosId = table.Column<int>(nullable: true),
                    VehiculoId = table.Column<int>(nullable: false),
                    PremiteCopia = table.Column<bool>(nullable: false),
                    OrdenId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenVehiculo", x => x.OrdenVehiculoId);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculo_Distribuidores_DistribuidorId",
                        column: x => x.DistribuidorId,
                        principalTable: "Distribuidores",
                        principalColumn: "DistribuidorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculo_PuntosInteres_LugarEntregaPuntoInteresId",
                        column: x => x.LugarEntregaPuntoInteresId,
                        principalTable: "PuntosInteres",
                        principalColumn: "PuntoInteresId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculo_PuntosInteres_LugarRecogidaPuntoInteresId",
                        column: x => x.LugarRecogidaPuntoInteresId,
                        principalTable: "PuntosInteres",
                        principalColumn: "PuntoInteresId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculo_Orden_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "Orden",
                        principalColumn: "OrdenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculo_PrecioRentaAutos_PrecioRentaAutosId",
                        column: x => x.PrecioRentaAutosId,
                        principalTable: "PrecioRentaAutos",
                        principalColumn: "PrecioRentaAutosId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenVehiculo_Productos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoCategoriaAuto_VehiculoProductoId",
                table: "VehiculoCategoriaAuto",
                column: "VehiculoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_ActividadId",
                table: "OrdenActividad",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_DistribuidorId",
                table: "OrdenActividad",
                column: "DistribuidorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_OrdenId",
                table: "OrdenActividad",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_PrecioActividadId",
                table: "OrdenActividad",
                column: "PrecioActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_AlojamientoId",
                table: "OrdenAlojamiento",
                column: "AlojamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_ConfiguracionVoucherId",
                table: "OrdenAlojamiento",
                column: "ConfiguracionVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_DistribuidorId",
                table: "OrdenAlojamiento",
                column: "DistribuidorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_HabitacionId",
                table: "OrdenAlojamiento",
                column: "HabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_OrdenId",
                table: "OrdenAlojamiento",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_PlanesAlimenticiosId",
                table: "OrdenAlojamiento",
                column: "PlanesAlimenticiosId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_PrecioAlojamientoId",
                table: "OrdenAlojamiento",
                column: "PrecioAlojamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_ConfiguracionVoucherId",
                table: "OrdenTraslado",
                column: "ConfiguracionVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_DistribuidorId",
                table: "OrdenTraslado",
                column: "DistribuidorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_LugarEntregaPuntoInteresId",
                table: "OrdenTraslado",
                column: "LugarEntregaPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_LugarRecogidaPuntoInteresId",
                table: "OrdenTraslado",
                column: "LugarRecogidaPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_OrdenId",
                table: "OrdenTraslado",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_PrecioTrasladoId",
                table: "OrdenTraslado",
                column: "PrecioTrasladoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTraslado_TrasladoProductoId",
                table: "OrdenTraslado",
                column: "TrasladoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_DistribuidorId",
                table: "OrdenVehiculo",
                column: "DistribuidorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_LugarEntregaPuntoInteresId",
                table: "OrdenVehiculo",
                column: "LugarEntregaPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_LugarRecogidaPuntoInteresId",
                table: "OrdenVehiculo",
                column: "LugarRecogidaPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_OrdenId",
                table: "OrdenVehiculo",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_PrecioRentaAutosId",
                table: "OrdenVehiculo",
                column: "PrecioRentaAutosId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_VehiculoId",
                table: "OrdenVehiculo",
                column: "VehiculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehiculoCategoriaAuto_Productos_VehiculoProductoId",
                table: "VehiculoCategoriaAuto",
                column: "VehiculoProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehiculoCategoriaAuto_Productos_VehiculoProductoId",
                table: "VehiculoCategoriaAuto");

            migrationBuilder.DropTable(
                name: "OrdenActividad");

            migrationBuilder.DropTable(
                name: "OrdenAlojamiento");

            migrationBuilder.DropTable(
                name: "OrdenTraslado");

            migrationBuilder.DropTable(
                name: "OrdenVehiculo");

            migrationBuilder.DropIndex(
                name: "IX_VehiculoCategoriaAuto_VehiculoProductoId",
                table: "VehiculoCategoriaAuto");

            migrationBuilder.DropColumn(
                name: "VehiculoProductoId",
                table: "VehiculoCategoriaAuto");

            migrationBuilder.DropColumn(
                name: "FechaPeticion",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "NombreClienteFinal",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "NombreOrden",
                table: "Orden");

            migrationBuilder.RenameColumn(
                name: "NumeroAsistencia",
                table: "Orden",
                newName: "Nombre");

            migrationBuilder.AddColumn<int>(
                name: "Duracion",
                table: "Orden",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OrdenProducto",
                columns: table => new
                {
                    OrdenProductoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrdenId = table.Column<int>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false)
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
                name: "IX_OrdenProducto_OrdenId",
                table: "OrdenProducto",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenProducto_ProductoId",
                table: "OrdenProducto",
                column: "ProductoId");
        }
    }
}
