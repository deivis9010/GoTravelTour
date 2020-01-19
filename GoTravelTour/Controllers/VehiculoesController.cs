using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using Microsoft.AspNetCore.Authorization;
using PagedList;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public VehiculoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Vehiculoes
        [HttpGet]
        public IEnumerable<Vehiculo> GetVehiculos (string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Vehiculo> lista;
            if (col == "-1")
            {
                lista= _context.Vehiculos
                    .Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                    .Include(v => v.TipoProducto)
                    .Include(v => v.ListaDistribuidoresProducto)
                    .OrderBy(a => a.Nombre)
                    .ToList();
               
                   
                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Vehiculos
                    .Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                    .Include(v => v.TipoProducto)
                    .Include(v => v.ListaDistribuidoresProducto)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
               
            }
            else
            {
                lista = _context.Vehiculos
                    .Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                    .Include(v => v.TipoProducto)
                    .Include(v => v.ListaDistribuidoresProducto)
                    .ToPagedList(pageIndex, pageSize).ToList();
              
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Nombre);

                        }




                    }

                    break;
            }

            return lista;
        }
        // GET: api/Vehiculoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetVehiculoCount()
        {
            return _context.Vehiculos.Count();
        }

        // GET: api/Vehiculoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo == null)
            {
                return NotFound();
            }

            return Ok(vehiculo);
        }

        // PUT: api/Vehiculoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutVehiculo([FromRoute] int id, [FromBody] Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehiculo.ProductoId)
            {
                return BadRequest();
            }

            _context.Entry(vehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVehiculo", new { id = vehiculo.ProductoId }, vehiculo);
        }

        // POST: api/Vehiculoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostVehiculo([FromBody] Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            vehiculo.SKU = u.GetSKUCodigo();
            _context.Vehiculos.Add(vehiculo);
           
                await _context.SaveChangesAsync();
            
            

            return CreatedAtAction("GetVehiculo", new { id = vehiculo.ProductoId }, vehiculo);
        }

        // DELETE: api/Vehiculoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return Ok(vehiculo);
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.ProductoId == id);
        }

        // GET: api/Vehiculoes/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(int idContrato = -1, int idDistribuidor = -1)
        {
            IEnumerable<Contrato> lista= new  List<Contrato>();            

            if (idContrato == -1 && idDistribuidor == -1)
            {
                
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {
                                               
                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId== contrato.DistribuidorId).ToList();
                        if( contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();
                                   
                                    j++;
                                }

                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }
                            

                        }
                        

                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor != -1)
            {
                
                 lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)                
                .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId == contrato.DistribuidorId).ToList();
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }
            else             
              if (idContrato != -1 && idDistribuidor == -1 )
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.ContratoId == idContrato )
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId == contrato.DistribuidorId).ToList();
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1 )
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.DistribuidorId == idDistribuidor )
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId == contrato.DistribuidorId).ToList();
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                  .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }


            return lista;

        }
    }
}