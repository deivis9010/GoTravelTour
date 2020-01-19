﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Servicio
    {
        public int ServicioId { get; set; }
        public string Nombre { get; set; }
        public bool Opcional { get; set; }
        public string Categoria { get; set; }
        public int HorasAdicionales { get; set; }
        public decimal? Incluido { get; set; }
        public decimal? PrecioAdulto { get; set; }
        public decimal? PrecioNino { get; set; }
        public decimal? PrecioInfante { get; set; }
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
        public Temporada Temporada { get; set; }

    }
}
