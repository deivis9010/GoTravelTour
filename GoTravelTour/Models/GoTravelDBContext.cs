using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class GoTravelDBContext : DbContext
    {
        public GoTravelDBContext(DbContextOptions<GoTravelDBContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<PlanesAlimenticios> PlanesAlimenticios { get; set; }
        public DbSet<AlmacenImagenes> AlmacenImagenes { get; set; }
        public DbSet<Rutas> Rutas { get; set; }
        public DbSet<Region> Regiones { get; set; }
        public DbSet<PuntoInteres> PuntosInteres { get; set; }

       

    }
}
