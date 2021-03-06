﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Comodidades
    {
        public int ComodidadesId { get; set; }
        public string Nombre { get; set; }
        public int CategoriaComodidadId { get; set; }
        public CategoriaComodidad CategoriaComodidad { get; set; }
        public List<ComodidadesProductos> ListaProductos { get; set; }
    }
}
