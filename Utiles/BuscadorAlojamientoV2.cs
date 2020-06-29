using GoTravelTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    /// <summary>
    /// Clase para hacer la busqueda especifica
    /// </summary>
    public class BuscadorAlojamientoV2
    {
        public Cliente Cliente { get; set; }
        public PlanesAlimenticios PlanAlimenticio { get; set; }
        public Alojamiento Alojamiento { get; set; }

        public TipoHabitacion TipoHabitacion { get; set; }
        public int CantidadAdultos { get; set; }
        public int CantidadMenores { get; set; }
        public int CantidadInfantes { get; set; }
        public int CantidadHabitaciones { get; set; }
        public string NombreHabitacion { get; set; }
        public Habitacion Habitacion { get; set; }

        public DateTime Entrada { get; set; }
        public DateTime Salida { get; set; }
    }
}
