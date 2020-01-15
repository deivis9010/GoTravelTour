﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class RestriccionesPrecio
    {
        public int RestriccionesPrecioId { get; set; }
        public double Precio { get; set; }
        public int RestriccionesId { get; set; }
        public Restricciones Restricciones { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
       
    }
}