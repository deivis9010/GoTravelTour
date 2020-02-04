﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Distribuidor
    {
        public int DistribuidorId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public bool IsActivo { get; set; }
        public List<ProductoDistribuidor> ListaProductosDistribuidos { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<Actividad> ListaActividades { get; set; }


    }
}