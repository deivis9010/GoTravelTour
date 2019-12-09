using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class TablasGenerales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comodidades",
                columns: table => new
                {
                    ComodidadesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comodidades", x => x.ComodidadesId);
                });

            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    ContratoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    IsActivo = table.Column<bool>(nullable: false),
                    OfertaEspecial = table.Column<bool>(nullable: false),
                    CodigoOfertaEspecial = table.Column<string>(nullable: true),
                    FechaInicioTravel = table.Column<DateTime>(nullable: true),
                    FechaFinTravel = table.Column<DateTime>(nullable: true),
                    FechaInicioBooking = table.Column<DateTime>(nullable: true),
                    FechaFinBooking = table.Column<DateTime>(nullable: true),
                    TipoProductoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.ContratoId);
                    table.ForeignKey(
                        name: "FK_Contratos_TipoProductos_TipoProductoId",
                        column: x => x.TipoProductoId,
                        principalTable: "TipoProductos",
                        principalColumn: "TipoProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Distribuidores",
                columns: table => new
                {
                    DistribuidorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distribuidores", x => x.DistribuidorId);
                });

            migrationBuilder.CreateTable(
                name: "PosibleCombinaciones",
                columns: table => new
                {
                    PosibleCombinacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CantInfantes = table.Column<int>(nullable: false),
                    CantNino = table.Column<int>(nullable: false),
                    CantAdult = table.Column<int>(nullable: false),
                    TipoHabitacionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosibleCombinaciones", x => x.PosibleCombinacionId);
                    table.ForeignKey(
                        name: "FK_PosibleCombinaciones_TipoHabitaciones_TipoHabitacionId",
                        column: x => x.TipoHabitacionId,
                        principalTable: "TipoHabitaciones",
                        principalColumn: "TipoHabitacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Correo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.ProveedorId);
                });

            migrationBuilder.CreateTable(
                name: "Temporadas",
                columns: table => new
                {
                    TemporadaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    ContratoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temporadas", x => x.TemporadaId);
                    table.ForeignKey(
                        name: "FK_Temporadas_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    SKU = table.Column<string>(nullable: true),
                    DescripcionCorta = table.Column<string>(nullable: true),
                    IsActivo = table.Column<bool>(nullable: false),
                    Latitud = table.Column<string>(nullable: true),
                    Longitud = table.Column<string>(nullable: true),
                    PermiteAdult = table.Column<bool>(nullable: false),
                    PermiteInfante = table.Column<bool>(nullable: false),
                    PermiteNino = table.Column<bool>(nullable: false),
                    PermiteHacerCopia = table.Column<bool>(nullable: false),
                    ProveedorId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    PuntoInteresId = table.Column<int>(nullable: false),
                    TipoProductoId = table.Column<int>(nullable: false),
                    Notas = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoId);
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_PuntosInteres_PuntoInteresId",
                        column: x => x.PuntoInteresId,
                        principalTable: "PuntosInteres",
                        principalColumn: "PuntoInteresId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_Regiones_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regiones",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_TipoProductos_TipoProductoId",
                        column: x => x.TipoProductoId,
                        principalTable: "TipoProductos",
                        principalColumn: "TipoProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RangoFechas",
                columns: table => new
                {
                    RangoFechasId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FechaInicio = table.Column<DateTime>(nullable: false),
                    FechaFin = table.Column<DateTime>(nullable: false),
                    TemporadaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangoFechas", x => x.RangoFechasId);
                    table.ForeignKey(
                        name: "FK_RangoFechas_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComodidadesProductos",
                columns: table => new
                {
                    ComodidadesProductosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: false),
                    ComodidadesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComodidadesProductos", x => x.ComodidadesProductosId);
                    table.ForeignKey(
                        name: "FK_ComodidadesProductos_Comodidades_ComodidadesId",
                        column: x => x.ComodidadesId,
                        principalTable: "Comodidades",
                        principalColumn: "ComodidadesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComodidadesProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Habitaciones",
                columns: table => new
                {
                    HabitacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SKU = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    IsActiva = table.Column<bool>(nullable: false),
                    IsPayPerRoom = table.Column<bool>(nullable: false),
                    TipoHabitacionId = table.Column<int>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habitaciones", x => x.HabitacionId);
                    table.ForeignKey(
                        name: "FK_Habitaciones_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Habitaciones_TipoHabitaciones_TipoHabitacionId",
                        column: x => x.TipoHabitacionId,
                        principalTable: "TipoHabitaciones",
                        principalColumn: "TipoHabitacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrecioActividades",
                columns: table => new
                {
                    PrecioActividadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HorasAdicionales = table.Column<int>(nullable: false),
                    Incluido = table.Column<double>(nullable: false),
                    PrecioAdulto = table.Column<double>(nullable: false),
                    PrecioNino = table.Column<double>(nullable: false),
                    PrecioInfante = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioActividades", x => x.PrecioActividadId);
                    table.ForeignKey(
                        name: "FK_PrecioActividades_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrecioActividades_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioActividades_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrecioComodidades",
                columns: table => new
                {
                    PrecioComodidadesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ComodidadesId = table.Column<int>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioComodidades", x => x.PrecioComodidadesId);
                    table.ForeignKey(
                        name: "FK_PrecioComodidades_Comodidades_ComodidadesId",
                        column: x => x.ComodidadesId,
                        principalTable: "Comodidades",
                        principalColumn: "ComodidadesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioComodidades_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrecioPlanesAlimenticios",
                columns: table => new
                {
                    PrecioPlanesAlimenticiosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Precio = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioPlanesAlimenticios", x => x.PrecioPlanesAlimenticiosId);
                    table.ForeignKey(
                        name: "FK_PrecioPlanesAlimenticios_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrecioPlanesAlimenticios_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioPlanesAlimenticios_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrecioRentaAutos",
                columns: table => new
                {
                    PrecioRentaAutosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DiasExtra = table.Column<int>(nullable: false),
                    Seguro = table.Column<double>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioRentaAutos", x => x.PrecioRentaAutosId);
                    table.ForeignKey(
                        name: "FK_PrecioRentaAutos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrecioRentaAutos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioRentaAutos_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoDistribuidores",
                columns: table => new
                {
                    ProductoDistribuidorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: false),
                    DistribuidorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoDistribuidores", x => x.ProductoDistribuidorId);
                    table.ForeignKey(
                        name: "FK_ProductoDistribuidores_Distribuidores_DistribuidorId",
                        column: x => x.DistribuidorId,
                        principalTable: "Distribuidores",
                        principalColumn: "DistribuidorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoDistribuidores_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestriccionesActividades",
                columns: table => new
                {
                    RestriccionesActividadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Minimo = table.Column<int>(nullable: false),
                    Maximo = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    IsActivo = table.Column<bool>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesActividades", x => x.RestriccionesActividadId);
                    table.ForeignKey(
                        name: "FK_RestriccionesActividades_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestriccionesActividades_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestriccionesActividades_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestriccionesRentasAutos",
                columns: table => new
                {
                    RestriccionesRentasAutosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Minimo = table.Column<int>(nullable: false),
                    Maximo = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    IsActivo = table.Column<bool>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesRentasAutos", x => x.RestriccionesRentasAutosId);
                    table.ForeignKey(
                        name: "FK_RestriccionesRentasAutos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestriccionesRentasAutos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestriccionesRentasAutos_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CombinacionHuespedes",
                columns: table => new
                {
                    CombinacionHuespedesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CantInfantes = table.Column<int>(nullable: false),
                    CantNino = table.Column<int>(nullable: false),
                    CantAdult = table.Column<int>(nullable: false),
                    IsActivo = table.Column<bool>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    HabitacionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombinacionHuespedes", x => x.CombinacionHuespedesId);
                    table.ForeignKey(
                        name: "FK_CombinacionHuespedes_Habitaciones_HabitacionId",
                        column: x => x.HabitacionId,
                        principalTable: "Habitaciones",
                        principalColumn: "HabitacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CombinacionHuespedes_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrecioAlojamiento",
                columns: table => new
                {
                    PrecioAlojamientoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Precio = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true),
                    TemporadaId = table.Column<int>(nullable: true),
                    HabitacionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioAlojamiento", x => x.PrecioAlojamientoId);
                    table.ForeignKey(
                        name: "FK_PrecioAlojamiento_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrecioAlojamiento_Habitaciones_HabitacionId",
                        column: x => x.HabitacionId,
                        principalTable: "Habitaciones",
                        principalColumn: "HabitacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrecioAlojamiento_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioAlojamiento_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Modificadores",
                columns: table => new
                {
                    ModificadorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdentificadorModificador = table.Column<string>(nullable: true),
                    TipoModificadorPrecio = table.Column<string>(nullable: true),
                    IsActivo = table.Column<bool>(nullable: false),
                    CantInfantes = table.Column<int>(nullable: false),
                    CantNino = table.Column<int>(nullable: false),
                    CantAdult = table.Column<int>(nullable: false),
                    FechaI = table.Column<DateTime>(nullable: true),
                    FechaF = table.Column<DateTime>(nullable: true),
                    ContratoId = table.Column<int>(nullable: false),
                    TipoProductoId = table.Column<int>(nullable: true),
                    PrecioAlojamientoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modificadores", x => x.ModificadorId);
                    table.ForeignKey(
                        name: "FK_Modificadores_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Modificadores_PrecioAlojamiento_PrecioAlojamientoId",
                        column: x => x.PrecioAlojamientoId,
                        principalTable: "PrecioAlojamiento",
                        principalColumn: "PrecioAlojamientoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Modificadores_TipoProductos_TipoProductoId",
                        column: x => x.TipoProductoId,
                        principalTable: "TipoProductos",
                        principalColumn: "TipoProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModificadorProductos",
                columns: table => new
                {
                    ModificadorProductosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModificadorId = table.Column<int>(nullable: true),
                    ProductoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModificadorProductos", x => x.ModificadorProductosId);
                    table.ForeignKey(
                        name: "FK_ModificadorProductos_Modificadores_ModificadorId",
                        column: x => x.ModificadorId,
                        principalTable: "Modificadores",
                        principalColumn: "ModificadorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModificadorProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reglas",
                columns: table => new
                {
                    ReglasId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TipoPrecioHabitacion = table.Column<string>(nullable: true),
                    TipoPersona = table.Column<string>(nullable: true),
                    PrecioFijo = table.Column<double>(nullable: false),
                    PrecioPorCiento = table.Column<double>(nullable: false),
                    IsActivo = table.Column<bool>(nullable: false),
                    ModificadorId = table.Column<int>(nullable: false),
                    TipoHabitacionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reglas", x => x.ReglasId);
                    table.ForeignKey(
                        name: "FK_Reglas_Modificadores_ModificadorId",
                        column: x => x.ModificadorId,
                        principalTable: "Modificadores",
                        principalColumn: "ModificadorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reglas_TipoHabitaciones_TipoHabitacionId",
                        column: x => x.TipoHabitacionId,
                        principalTable: "TipoHabitaciones",
                        principalColumn: "TipoHabitacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CombinacionHuespedes_HabitacionId",
                table: "CombinacionHuespedes",
                column: "HabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CombinacionHuespedes_ProductoId",
                table: "CombinacionHuespedes",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComodidadesProductos_ComodidadesId",
                table: "ComodidadesProductos",
                column: "ComodidadesId");

            migrationBuilder.CreateIndex(
                name: "IX_ComodidadesProductos_ProductoId",
                table: "ComodidadesProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_TipoProductoId",
                table: "Contratos",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_ProductoId",
                table: "Habitaciones",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_TipoHabitacionId",
                table: "Habitaciones",
                column: "TipoHabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_ContratoId",
                table: "Modificadores",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_PrecioAlojamientoId",
                table: "Modificadores",
                column: "PrecioAlojamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_TipoProductoId",
                table: "Modificadores",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ModificadorProductos_ModificadorId",
                table: "ModificadorProductos",
                column: "ModificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ModificadorProductos_ProductoId",
                table: "ModificadorProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PosibleCombinaciones_TipoHabitacionId",
                table: "PosibleCombinaciones",
                column: "TipoHabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioActividades_ContratoId",
                table: "PrecioActividades",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioActividades_ProductoId",
                table: "PrecioActividades",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioActividades_TemporadaId",
                table: "PrecioActividades",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioAlojamiento_ContratoId",
                table: "PrecioAlojamiento",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioAlojamiento_HabitacionId",
                table: "PrecioAlojamiento",
                column: "HabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioAlojamiento_ProductoId",
                table: "PrecioAlojamiento",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioAlojamiento_TemporadaId",
                table: "PrecioAlojamiento",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioComodidades_ComodidadesId",
                table: "PrecioComodidades",
                column: "ComodidadesId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioComodidades_ProductoId",
                table: "PrecioComodidades",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioPlanesAlimenticios_ContratoId",
                table: "PrecioPlanesAlimenticios",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioPlanesAlimenticios_ProductoId",
                table: "PrecioPlanesAlimenticios",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioPlanesAlimenticios_TemporadaId",
                table: "PrecioPlanesAlimenticios",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioRentaAutos_ContratoId",
                table: "PrecioRentaAutos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioRentaAutos_ProductoId",
                table: "PrecioRentaAutos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioRentaAutos_TemporadaId",
                table: "PrecioRentaAutos",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoDistribuidores_DistribuidorId",
                table: "ProductoDistribuidores",
                column: "DistribuidorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoDistribuidores_ProductoId",
                table: "ProductoDistribuidores",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_PuntoInteresId",
                table: "Productos",
                column: "PuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_RegionId",
                table: "Productos",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TipoProductoId",
                table: "Productos",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RangoFechas_TemporadaId",
                table: "RangoFechas",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglas_ModificadorId",
                table: "Reglas",
                column: "ModificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglas_TipoHabitacionId",
                table: "Reglas",
                column: "TipoHabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesActividades_ContratoId",
                table: "RestriccionesActividades",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesActividades_ProductoId",
                table: "RestriccionesActividades",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesActividades_TemporadaId",
                table: "RestriccionesActividades",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesRentasAutos_ContratoId",
                table: "RestriccionesRentasAutos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesRentasAutos_ProductoId",
                table: "RestriccionesRentasAutos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesRentasAutos_TemporadaId",
                table: "RestriccionesRentasAutos",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Temporadas_ContratoId",
                table: "Temporadas",
                column: "ContratoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CombinacionHuespedes");

            migrationBuilder.DropTable(
                name: "ComodidadesProductos");

            migrationBuilder.DropTable(
                name: "ModificadorProductos");

            migrationBuilder.DropTable(
                name: "PosibleCombinaciones");

            migrationBuilder.DropTable(
                name: "PrecioActividades");

            migrationBuilder.DropTable(
                name: "PrecioComodidades");

            migrationBuilder.DropTable(
                name: "PrecioPlanesAlimenticios");

            migrationBuilder.DropTable(
                name: "PrecioRentaAutos");

            migrationBuilder.DropTable(
                name: "ProductoDistribuidores");

            migrationBuilder.DropTable(
                name: "RangoFechas");

            migrationBuilder.DropTable(
                name: "Reglas");

            migrationBuilder.DropTable(
                name: "RestriccionesActividades");

            migrationBuilder.DropTable(
                name: "RestriccionesRentasAutos");

            migrationBuilder.DropTable(
                name: "Comodidades");

            migrationBuilder.DropTable(
                name: "Distribuidores");

            migrationBuilder.DropTable(
                name: "Modificadores");

            migrationBuilder.DropTable(
                name: "PrecioAlojamiento");

            migrationBuilder.DropTable(
                name: "Habitaciones");

            migrationBuilder.DropTable(
                name: "Temporadas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "Proveedores");
        }
    }
}
