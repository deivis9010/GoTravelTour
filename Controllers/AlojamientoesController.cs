using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using Microsoft.AspNetCore.Authorization;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlojamientoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public AlojamientoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Alojamientoes
        [HttpGet]
        public IEnumerable<Alojamiento> GetAlojamientos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Alojamiento> lista;
            if (col == "-1")
            {
                lista = _context.Alojamientos
                    //.Include(a => a.ListaComodidades)
                    //.Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                   // .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)
                   // .Include(a => a.PuntoInteres)
                   // .Include(a => a.CategoriaHoteles)
                   // .Include(a => a.ListaPlanesAlimenticios)
                    .OrderBy(a => a.Nombre)
                    .ToList();

              

                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Alojamientos
                    //.Include(a => a.ListaComodidades)
                   // .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                   // .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)                   
                   // .Include(a => a.PuntoInteres)
                   //.Include(a => a.CategoriaHoteles)
                   // .Include(a => a.ListaPlanesAlimenticios)
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.Alojamientos
                    //.Include(a => a.ListaComodidades)
                   // .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                   // .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)
                    //.Include(a => a.PuntoInteres)
                    //.Include(a => a.CategoriaHoteles)
                   // .Include(a => a.ListaPlanesAlimenticios)
                    .OrderBy(a => a.Nombre)
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
        // GET: api/Alojamientoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetAlojamientoesCount()
        {
            return _context.Alojamientos.Count();
        }

        // GET: api/Alojamientoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*var alojamiento = await _context.Alojamientos
                .FindAsync(id);*/
                var alojamiento =  _context.Alojamientos
                 .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)
                    .Include(a => a.PuntoInteres)
                    .Include(a => a.CategoriaHoteles)
                    .Include(a => a.ListaPlanesAlimenticios).Single(x=>x.ProductoId==id);

            if (alojamiento == null)
            {
                return NotFound();
            }

            return Ok(alojamiento);
        }

        // PUT: api/Alojamientoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAlojamiento([FromRoute] int id, [FromBody] Alojamiento alojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != alojamiento.ProductoId)
            {
                return BadRequest();
            }

            alojamiento.Proveedor = _context.Proveedores.First(x => x.ProveedorId == alojamiento.ProveedorId);
            if (alojamiento.PuntoInteres != null && alojamiento.PuntoInteres.PuntoInteresId > 0)
                alojamiento.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == alojamiento.PuntoInteres.PuntoInteresId);
            alojamiento.TipoAlojamiento = _context.TipoAlojamientos.First(x => x.TipoAlojamientoId == alojamiento.TipoAlojamientoId);
            alojamiento.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == alojamiento.TipoProductoId);
            if (alojamiento.CategoriaHotelesId > 0)
                alojamiento.CategoriaHoteles = _context.CategoriaHoteles.First(x => x.CategoriaHotelesId == alojamiento.CategoriaHotelesId);
            
            List<ProductoDistribuidor> distribuidors = _context.ProductoDistribuidores.Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
            foreach (var item in distribuidors)
            {
                _context.ProductoDistribuidores.Remove(item);
            }
            List<ComodidadesProductos> comodidades = _context.ComodidadesProductos.Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
            foreach (var item in comodidades)
            {
                _context.ComodidadesProductos.Remove(item);
            }

            List<AlojamientosPlanesAlimenticios> planes = _context.AlojamientosPlanesAlimenticios.Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
            foreach (var item in planes)
            {
                _context.AlojamientosPlanesAlimenticios.Remove(item);
            }

            foreach (var item in alojamiento.ListaDistribuidoresProducto)
            {
                item.ProductoId = alojamiento.ProductoId;
                _context.ProductoDistribuidores.Add(item);
            }
            foreach (var item in alojamiento.ListaComodidades)
            {
                item.ProductoId = alojamiento.ProductoId;
                _context.ComodidadesProductos.Add(item);

            }
            foreach (var item in alojamiento.ListaPlanesAlimenticios)
            {
                item.ProductoId = alojamiento.ProductoId;
                _context.AlojamientosPlanesAlimenticios.Add(item);

            }
            _context.Entry(alojamiento).State = EntityState.Modified;




            try
            {
                await _context.SaveChangesAsync();
                
                

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlojamientoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(alojamiento);
        }

        // POST: api/Alojamientoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostAlojamiento([FromBody] Alojamiento alojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            alojamiento.SKU = u.GetSKUCodigo();
            alojamiento.Proveedor = _context.Proveedores.First(x => x.ProveedorId == alojamiento.ProveedorId);
            if(alojamiento.PuntoInteres!= null && alojamiento.PuntoInteres.PuntoInteresId > 0)
            alojamiento.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == alojamiento.PuntoInteres.PuntoInteresId);
            alojamiento.TipoAlojamiento = _context.TipoAlojamientos.First(x => x.TipoAlojamientoId == alojamiento.TipoAlojamientoId);
            alojamiento.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == alojamiento.TipoProductoId);
            if(alojamiento.CategoriaHotelesId > 0)
            alojamiento.CategoriaHoteles = _context.CategoriaHoteles.First(x => x.CategoriaHotelesId == alojamiento.CategoriaHotelesId);
            _context.Alojamientos.Add(alojamiento);
            await _context.SaveChangesAsync();
            alojamiento.ListaPlanesAlimenticios = _context.AlojamientosPlanesAlimenticios.Include(x => x.PlanesAlimenticios).Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
            return CreatedAtAction("GetAlojamiento", new { id = alojamiento.ProductoId }, alojamiento);
        }

        // DELETE: api/Alojamientoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var alojamiento = await _context.Alojamientos.FindAsync(id);
            if (alojamiento == null)
            {
                return NotFound();
            }

            _context.Alojamientos.Remove(alojamiento);
            await _context.SaveChangesAsync();

            return Ok(alojamiento);
        }

        private bool AlojamientoExists(int id)
        {
            return _context.Alojamientos.Any(e => e.ProductoId == id);
        }




        // GET: api/Alojamientoes/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(int idContrato = -1, int idDistribuidor = -1)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.TipoProducto.Nombre == "Accommodation")
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                            .Include(x => x.Producto)
                            .ThenInclude(x => x.TipoProducto)
                            .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Accommodation").ToList();

                       

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                               
                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                {
                                        if (_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp = new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }
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
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == "Accommodation")
               .OrderBy(a => a.Nombre)
               .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                            .Include(x => x.Producto)
                            .ThenInclude(x => x.TipoProducto)
                            .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Accommodation").ToList();

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                           .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                    foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                    {
                                        if (_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp = new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }
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
              if (idContrato != -1 && idDistribuidor == -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == "Accommodation")
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                            .Include(x => x.Producto)
                            .ThenInclude(x => x.TipoProducto)
                            .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Accommodation").ToList();

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                   .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                    foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                    {
                                        if (_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp = new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }
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
              if (idContrato == -1 && idDistribuidor != -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == "Accommodation")
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                            .Include(x => x.Producto)
                            .ThenInclude(x => x.TipoProducto)
                            .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Accommodation").ToList();

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                   .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                    foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                    {
                                        if(_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp =new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }
                                        
                                        
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