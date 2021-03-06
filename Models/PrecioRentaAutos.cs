﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioRentaAutos
    {
        public int PrecioRentaAutosId { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal DiasExtra { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Seguro { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Deposito { get; set; }
        //public bool IsActivo { get; set; }
       //public int ContratoId { get; set; }
        //public int TemporadaId { get; set; }
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Auto { get; set; }
        //public Contrato Contrato { get; set; }
        public Temporada Temporada { get; set; }
    }

}
