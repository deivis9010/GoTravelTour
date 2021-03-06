﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenActividad
    {
        public int OrdenActividadId { get; set; }
        public DateTime FechaActividad { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? CantAdulto { get; set; }
        public int? CantNino { get; set; }
        public int? CantInfante { get; set; }
        public string TelefonoReferencia { get; set; }
        public string DireccionReferencia { get; set; }
        public string NombreCliente { get; set; }
        public string VenueName { get; set; } //que es
        public string NumeroConfirmacion { get; set; }
        public string DescripcionServicio { get; set; }
        public string NotasAdicionales { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string ServiciosExcluidos { get; set; } //iran los id de los servicios q no son opcionales y no se incluirarn en la orden ejemplo (2525-3030-)
        public int IdBillQB { get; set; } //id del estimado creado en cQB  para poder editarlo si esta en null es  pq a la orden no se le ha creado el estimado
        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public ConfiguracionVoucher Voucher { get; set; }
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public int ActividadId { get; set; }
        public Actividad Actividad { get; set; }
        public PrecioActividad PrecioActividad { get; set; }
       // public List<PrecioServicio> Servicios { get; set; }
        public bool PremiteCopia { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioOrden { get; set; }
        public Region LugarActividad { get; set; }
        public PuntoInteres LugarRecogida { get; set; }
        public PuntoInteres LugarRetorno { get; set; }
        public Sobreprecio Sobreprecio { get; set; }
        public decimal ValorSobreprecioAplicado { get; set; }

    }
}
